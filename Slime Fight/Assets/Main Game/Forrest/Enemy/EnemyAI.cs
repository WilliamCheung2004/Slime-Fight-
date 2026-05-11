using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public class EnemyAI : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemyHandler enemyHandler;

    private Spawner spawner;
    //[SerializeField] private Transform firePoint; 
    //[SerializeField] private GameObject projectilePrefab;

    [Header("Layers")]
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private LayerMask playerLayerMask;

    [Header("Detection Range")]
    [SerializeField] private float vision = 20f;
    [SerializeField] private float range = 12f;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private float correctAngle;
    [SerializeField] private BoxCollider enemyBounds;

    [Header("Enemy")]
    private GameObject[] enemies;
    private bool enemyMovingBack = false;
    private bool dead = false;
    [SerializeField] EnemyHandler enemy;

    [Header("Enemy Area")]
    public Vector3 areaCenter;
    public Vector3 areaSize;

    [Header("Enemy Vision")]
    private bool isPlayerVisible;
    private bool isPlayerInRange;
    public Vector3 originalPos;

    [Header("Boundary Check")]
    private bool isTouchingBoundary;
    private float buffer = 0.01f;
    private float secondsTouchedBoundary = 0f;
    private float touchingLimit = 2f;

    [Header("Enemy Slider")]
    [SerializeField] private UnityEngine.UI.Slider healthSlider;

    [Header("Player")]
    [SerializeField] private PlayerResource player;
    [SerializeField] private UpgradeManager playerUpgrade;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Idle", true);

        player = GameObject.FindGameObjectWithTag("Player Resources").GetComponent<PlayerResource>();
        playerUpgrade = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<UpgradeManager>();
    }
    private void Awake()
    {
        if (navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();

        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyBounds = GameObject.FindGameObjectsWithTag("EnemyLimit")[0].GetComponent<BoxCollider>();
        spawner = GameObject.Find("Spawner")?.GetComponent<Spawner>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            UnityEngine.Debug.LogError("EnemyAI: Player not found! Make sure the Spawner has a player assigned.");
        }

        animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        isTouchingBoundary = false;

        Vector3 areaCenter = enemyBounds.bounds.center;
        Vector3 areaSize = enemyBounds.bounds.size;

        if (!enemyMovingBack)
        {
            float minX = areaCenter.x - areaSize.x / 2f + buffer;
            float maxX = areaCenter.x + areaSize.x / 2f - buffer;
            float minZ = areaCenter.z - areaSize.z / 2f + buffer;
            float maxZ = areaCenter.z + areaSize.z / 2f - buffer;

            if (pos.z < minZ)
                isTouchingBoundary = true;
            else if (pos.z > maxZ)
                isTouchingBoundary = true;

            float clampMinX = areaCenter.x - areaSize.x / 2f;
            float clampMaxX = areaCenter.x + areaSize.x / 2f;
            float clampMinZ = areaCenter.z - areaSize.z / 2f;
            float clampMaxZ = areaCenter.z + areaSize.z / 2f;

            pos.x = Mathf.Clamp(pos.x, clampMinX, clampMaxX);
            pos.z = Mathf.Clamp(pos.z, clampMinZ, clampMaxZ);

            transform.position = pos;
        }

        if (isTouchingBoundary)
        {
            secondsTouchedBoundary += Time.deltaTime;
            if (secondsTouchedBoundary >= touchingLimit)
            {
                navAgent.SetDestination(originalPos);
                secondsTouchedBoundary = 0f;
                enemyMovingBack = true;
            }
        }
        else
        {
            secondsTouchedBoundary = 0f;
        }

        if (enemyMovingBack)
        {
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude < 0.01f)
                {
                    enemyMovingBack = false;
                    enemyHandler.SetCurrentHealth(enemyHandler.GetMaxHealth());
                    animator.SetBool("Idle", true);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Attack", false);
                }
            }
        }

    }
    private void FacePlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f; if (direction.sqrMagnitude < 0.01f) return;
        Quaternion targetRotation = Quaternion.LookRotation(-direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
    }

    private void FaceOriginalPos()
    {
        Vector3 direction = originalPos - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(-direction); ;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * 8f
        );
    }

    void Update()
    {
        Detect();
        UpdateState();

        // Rotate based on current state
        if (enemyMovingBack)
        {
            FaceOriginalPos();
        }
        else
        {
            FacePlayer();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, vision);
    }

    private void Detect()
    {

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        isPlayerVisible = distanceToPlayer <= vision;
        isPlayerInRange = distanceToPlayer <= range;

        if (isPlayerVisible && !isPlayerInRange)
        {
        }
        else if (isPlayerVisible && isPlayerInRange)
        {
        }
    }

    private void Chase()
    {
        if (playerTransform != null)
        {
            navAgent.SetDestination(playerTransform.position);
            animator.SetBool("Walk", true);
            animator.SetBool("Idle", false);
            animator.SetBool("Attack", false);
        }
    }

    private void Attack()
    {
        // Stop moving
        navAgent.SetDestination(transform.position);

        // Animation
        animator.SetBool("Attack", true);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
    }

    public void Damage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, playerLayerMask);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                player.TakeDamage(enemyHandler.GetDamage());
                SoundManager.SelectSound(SoundType.ENEMY1, 0, 0.25f);
            }
        }
    }

    private void Death()
    {
        if (animator != null)
            animator.SetTrigger("Die");

        if (navAgent != null)
            navAgent.enabled = false;

        playerUpgrade.GetMoneyEfficiency();
        float random = Random.Range(0f, 1f);

        player.SetMoney(player.GetMoney() + enemy.GetMoneyDrop());

        if (random <= playerUpgrade.GetMoneyEfficiency())
        {
            int bonusMoney = enemy.GetMoneyDrop() * 2;
            player.SetMoney(player.GetMoney() + bonusMoney);
            Debug.Log("Money Efficiency Bonus! Original: " + enemy.GetMoneyDrop() + ", Bonus: " + bonusMoney);
        }

        if (healthSlider != null)
            Destroy(healthSlider.gameObject);

        Destroy(gameObject, 2f);

    }

    private void UpdateState()
    {
        if (dead) return;

        if (!enemyMovingBack && !dead)
        {
            if (isPlayerVisible && !isPlayerInRange)
            {
                Chase();
            }

            else if (isPlayerVisible && isPlayerInRange)
            {
                Attack();
            }
        }
        if (enemyHandler.GetCurrentHealth() <= 0)
        {
            dead = true;
            spawner.EnemysKilled();
            Death();
        }
    }
}
