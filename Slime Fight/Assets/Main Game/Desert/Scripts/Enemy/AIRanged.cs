using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class AIRanged : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyHandler enemyHandler;
    [SerializeField] private Animator animator;
    [SerializeField] private Slider healthSlider;

    [Header("Player")]
    private Transform player;
    private PlayerResource playerResource;
    private UpgradeManager upgradeManager;
    private Collider playerCollider;

    [Header("Detection")]
    [SerializeField] private float visionRange = 20f;
    [SerializeField] private float attackRange = 10f;

    [Header("Attack")]
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 15f;

    [Header("Retreat")]
    [SerializeField] private float retreatDistance;
    [SerializeField] private float retreatCooldown;
    [SerializeField] private float retreatStrength;
    private Vector3 retreatDestination;

    private Spawner spawner;

    private float retreatTimer;
    private bool isRetreating;

    private float attackTimer;

    private Vector3 originalPosition;
    private bool dead;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = true;
        agent.updateRotation = false;

        originalPosition = transform.position;
        agent.stoppingDistance = attackRange;
    }

    private IEnumerator Start()
    {
        while (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
                playerCollider = p.GetComponent<Collider>();
            }

            yield return null;
        }


        while (playerResource == null)
        {
            GameObject pr = GameObject.FindGameObjectWithTag("Player Resources");
            if (pr != null)
                playerResource = pr.GetComponent<PlayerResource>();

            yield return null;
        }

        while (upgradeManager == null)
        {
            GameObject gm = GameObject.FindGameObjectWithTag("Game Manager");
            if (gm != null)
                upgradeManager = gm.GetComponent<UpgradeManager>();
            yield return null;
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
        if (dead)
            return;

        if (player == null)
            return;

        CheckHealth();

        if (!isRetreating)
            retreatTimer += Time.deltaTime;

        float currentDist = Vector3.Distance(transform.position, player.position);
        if (currentDist < retreatDistance && retreatTimer >= retreatCooldown)
        {
            isRetreating = true;
            retreatTimer = 0f;

            Vector3 dir = (transform.position - player.position).normalized;
            retreatDestination = transform.position + dir * retreatStrength;
        }


        if (isRetreating)
        {
            Retreat(currentDist);
            return;
        }


        if (dead || player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (distance <= visionRange)
        {
            Chase();
            animator.SetBool("Walk", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            Idle();
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
        }

        FaceTarget();
    }

    private bool HasLineOfSight()
    {
        if (playerCollider == null) return false;

        Vector3 origin = firePoint.position;

        Vector3 target = playerCollider.bounds.center;

        Vector3 direction = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        Debug.DrawLine(origin, target, Color.red, 0.1f);


        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    private void PlayHitSound()
    {
        SoundManager.SelectSound(SoundType.ENEMY2, 0);
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

    private void Retreat(float distance)
    {
        if (!CanUseAgent()) return;

        agent.stoppingDistance = 0f;
        agent.SetDestination(retreatDestination);

        animator.SetBool("Walk", true);

        FaceTarget();

        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            isRetreating = false;
            agent.stoppingDistance = attackRange;
        }
    }

    private void Attack()
    {
        if (!CanUseAgent()) return;
        if (isRetreating) return;

        agent.ResetPath();

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            if (HasLineOfSight())
            {
                Debug.Log("Has Sight");
                animator.SetBool("Walk", false);
                animator.SetBool("Idle", false);
                animator.SetTrigger("Attack");
                attackTimer = 0f;
            }
            else
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Idle", true);
            }
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Fireball>().Initialise(enemyHandler);

        Vector3 direction = (player.position - firePoint.position).normalized;

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
            SoundManager.SelectSound(SoundType.ENEMY2, 1);
        }

        Destroy(projectile, 5f);
    }

    private void Idle()
    {
        if (!CanUseAgent()) return;
        agent.SetDestination(transform.position);
    }

    private void FaceTarget()
    {
        Vector3 target;

        if (isRetreating)
        {
            target = transform.position + (transform.position - player.position);
        }
        else
        {
            target = player.position;
        }

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
            animator.SetTrigger("Die");
            Die();
        }
    }

    private void Die()
    {
        if (dead || playerResource == null)
            return;

        dead = true;

        if (agent != null && agent.enabled)
            agent.enabled = false;

        spawner.EnemysKilled();

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
}