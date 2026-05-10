using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System;

using Random = UnityEngine.Random;
public enum BossMode
{
    Melee,
    Ranged
}
public class BossAI : MonoBehaviour
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
    private bool canStart = false;
    private BossMode currentMode = BossMode.Melee;
    [SerializeField] private float modeSwitchInterval = 20f;
    private float modeSwitchTime = 0f;

    [Header("Damage")]
    [SerializeField] private float damageInterval = 1f;
    private float damageTimer = 0f;

    [Header("Dash")]
    private bool isDashing = false;
    private float lastAttackTime = 0f;

    [Header("Ranged Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float rangedAttackInterval = 2f;
    private float rangedAttackTimer = 0f;

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

        if (!canStart)
        {
            return;
        }

        if (agent.isOnOffMeshLink)
        {
            StartCoroutine(HandleOffMeshLink());
            return;
        }

        CheckHealth();
        lastAttackTime += Time.deltaTime;

        //Dash
        if (lastAttackTime > 5f && !isDashing && currentMode == BossMode.Melee)
        {
            lastAttackTime = 0f;
            StartCoroutine(Dash());
        }

        modeSwitchTime += Time.deltaTime;


        if (modeSwitchTime >= modeSwitchInterval)
        {
            SwitchMode();
            modeSwitchTime = 0f;
        }


        float distance = Vector3.Distance(transform.position, player.position);

        if (!isDashing)
        {
            if (currentMode == BossMode.Melee)
            {
                if (distance <= attackRange)
                {
                    Attack();
                    animator.SetBool("Walk", false);
                    animator.SetBool("Idle", false);
                    animator.SetBool("Attack", true);
                    animator.SetBool("AttackRanged", false);
                }
                else if (distance <= visionRange)
                {
                    Chase();
                    animator.SetBool("Walk", true);
                    animator.SetBool("Idle", false);
                    animator.SetBool("Attack", false);
                    animator.SetBool("AttackRanged", false);
                    damageTimer = 0f;
                }
                else
                {
                    Idle();
                    animator.SetBool("Walk", false);
                    animator.SetBool("Idle", true);
                    animator.SetBool("Attack", false);
                    animator.SetBool("AttackRanged", false);
                }

            }
            else
            {
                if (currentMode == BossMode.Ranged)
                {
                    if (distance <= attackRange)
                    {
                        RangedAttack();
                        animator.SetBool("Walk", false);
                        animator.SetBool("Idle", false);
                        animator.SetBool("Attack", false);
                        animator.SetBool("AttackRanged", true);
                    }
                    else if (distance <= visionRange)
                    {
                        Chase();
                        animator.SetBool("Walk", true);
                        animator.SetBool("Idle", false);
                        animator.SetBool("Attack", false);
                        animator.SetBool("AttackRanged", false);
                        damageTimer = 0f;
                    }
                    else
                    {
                        Idle();
                        animator.SetBool("Walk", false);
                        animator.SetBool("Idle", true);
                        animator.SetBool("Attack", false);
                        animator.SetBool("AttackRanged", false);
                    }

                }
            }
        }
        FaceTarget();
    }

    private void RangedAttack()
    {
        animator.SetTrigger("AttackRanged");
        lastAttackTime = 0f;
    }

    private void SwitchMode()
    {
        if (currentMode == BossMode.Melee)
        {
            currentMode = BossMode.Ranged;
            attackRange = 12f;
            agent.stoppingDistance = attackRange;
            Debug.Log("Boss switched to RANGED mode");
        }
        else
        {
            currentMode = BossMode.Melee;
            attackRange = 2.5f;
            agent.stoppingDistance = attackRange;
            Debug.Log("Boss switched to MELEE mode");
        }
    }


    private bool CanStart()
    {
        canStart = true;
        agent.enabled = true;
        return canStart;
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
        lastAttackTime = 0f;
        agent.ResetPath();
    }

    private void DealDamage()
    {
        if (playerResource != null)
            playerResource.TakeDamage(enemyHandler.GetDamage());
    }

    private void Idle()
    {
        if (!CanUseAgent()) return;
        agent.SetDestination(transform.position);
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

    public void TakeDamage(int dmg)
    {
        if (dead) return;

        if (enemyHandler.GetCurrentHealth() <= 0)
        {
            animator.SetTrigger("Die");
            Die();
        }
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
        if (dead) return;
        if (!playerResource) return;

        dead = true;

        if (agent != null && agent.enabled)
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

        // Re-enable agent movement
        agent.CompleteOffMeshLink();
        agent.updatePosition = true;
    }

    private IEnumerator Dash()
    {
        Debug.Log("DASHING");

        if (!CanUseAgent()) yield break;

        float originalAcceleration = agent.acceleration;
        float originalSpeed = agent.speed;
        float maxDashSpeed = 5f;
        float acceleration = 10f;
        float dashTimer = 0f;
        float maxDashDuration = 2f;

        isDashing = true;
        agent.acceleration = 100f;
        agent.autoBraking = false;

        float currentSpeed = originalSpeed;

        while (Vector3.Distance(transform.position, player.position) > 0.5f)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= maxDashDuration) break;

            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, originalSpeed, maxDashSpeed);
            agent.speed = currentSpeed;

            agent.SetDestination(player.position);

            if (Vector3.Distance(transform.position, player.position) < attackRange + 0.1f)
            {
                playerResource.TakeDamage(20);
                Debug.Log("Dash Damage Taken");
                break;
            }

            yield return null;
        }

        agent.speed = originalSpeed;
        agent.autoBraking = true;
        agent.acceleration = originalAcceleration;
        isDashing = false;
        lastAttackTime = 0f;
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
        }

        Destroy(projectile, 5f);
    }

    private void PlayScream()
    {
        SoundManager.SelectSound(SoundType.BOSS, 0);
    }

    private void PlayMeeleAttack()
    {
        SoundManager.SelectSound(SoundType.BOSS, 1);
    }

    private void PlayRangedAttack()
    {
        SoundManager.SelectSound(SoundType.BOSS, 2);
    }
}