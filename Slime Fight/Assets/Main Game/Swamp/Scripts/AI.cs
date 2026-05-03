using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class AI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyHandler enemyHandler;
    [SerializeField] private Animator animator;

    [Header("Player")]
    private Transform player;
    private PlayerResource playerResource;
    private UpgradeManager upgradeManager;

    [Header("Detection")]
    [SerializeField] private float visionRange = 20f;
    [SerializeField] private float attackRange = 3f;

    [Header("Movement")]
    public Vector3 originalPos;
    private bool returningHome;

    [Header("State")]
    private bool dead;

    [Header("Damage")]
    [SerializeField] private float damageInterval = 1f;
    private float damageTimer = 0f;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;

    private Spawner spawner;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = true;
        agent.updateRotation = false;

        originalPos = transform.position;
        agent.stoppingDistance = attackRange;
    }

    private IEnumerator Start()
    {
        while (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;

            yield return null;
        }

        upgradeManager = GameObject.FindGameObjectWithTag("Game Manager")?.GetComponent<UpgradeManager>();
        playerResource = GameObject.FindGameObjectWithTag("Player Resources")?.GetComponent<PlayerResource>();
        if (!playerResource)
        {
            Debug.Log("PRESOURCE NOT FOUND");
        }

        while (spawner == null)
        {
            GameObject sp = GameObject.FindGameObjectWithTag("Spawner");
            if (sp != null)
                spawner = sp.GetComponent<Spawner>();
            yield return null;
        }
    }

    private void Update()
    {
        if (dead || player == null)
            return;

        if (agent.isOnOffMeshLink)
        {
            StartCoroutine(HandleOffMeshLink());
            return;
        }

        CheckHealth();

        float distance = Vector3.Distance(transform.position, player.position);

        if (returningHome)
        {
            ReturnHome();
            return;
        }

        if (distance <= attackRange)
        {
            Attack();
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Attack", true);
        }
        else if (distance <= visionRange)
        {
            Chase();
            animator.SetBool("Walk", true);
            animator.SetBool("Idle", false);
            animator.SetBool("Attack", false);
            damageTimer = 0f;
        }
        else
        {
            Idle();
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
            animator.SetBool("Attack", false);
        }

        FaceTarget();
    }

    private bool CanUseAgent()
    {
        return agent != null && agent.enabled && agent.isOnNavMesh;
    }

    private void Chase()
    {
        if (!CanUseAgent()) return;
        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        if (!CanUseAgent()) return;

        agent.ResetPath();
    }

    private void Idle()
    {
        if (!CanUseAgent()) return;
        agent.SetDestination(transform.position);
    }

    private void ReturnHome()
    {
        if (!CanUseAgent()) return;

        agent.SetDestination(originalPos);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            returningHome = false;
        }
    }

    private void FaceTarget()
    {
        Vector3 target = returningHome ? originalPos : player.position;

        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
    }

    private void CheckHealth()
    {
        if (dead) return;

        if (enemyHandler.GetCurrentHealth() <= 0)
        {
            Debug.Log("EnemyDead");
            animator.SetTrigger("Die");
            Die();
        }
    }

    private void DealDamage()
    {
        if (playerResource != null)
            playerResource.TakeDamage(enemyHandler.GetDamage());
    }

    private void Die()
    {
        if (dead) return;
        if (!playerResource){ Debug.Log("PlayerResource not found"); return; }
        if (!spawner) { Debug.Log("Spawner not found"); return; }

        spawner.EnemysKilled();
        dead = true;
        agent.enabled = false;
        playerResource.SetMoney(playerResource.GetMoney() + enemyHandler.GetMoneyDrop());

        float random = Random.Range(0f, 1f);
        if (random <= upgradeManager.GetMoneyEfficiency())
        {
            int bonusMoney = enemyHandler.GetMoneyDrop();
            playerResource.SetMoney(playerResource.GetMoney() + bonusMoney);
        }

        if (healthSlider != null)
            Destroy(healthSlider.gameObject);

        Destroy(gameObject, 2f);
    }

    private IEnumerator HandleOffMeshLink()
    {

        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos;

        float duration = 1.0f;
        float t = 0f;

        agent.updatePosition = false;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float height = 2f; 
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * height;

            agent.transform.position = pos;

            yield return null;
        }

        agent.CompleteOffMeshLink();
        agent.updatePosition = true;
    }

}