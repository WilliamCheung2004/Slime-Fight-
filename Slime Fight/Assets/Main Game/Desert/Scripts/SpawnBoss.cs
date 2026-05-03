using UnityEngine;
using UnityEngine.AI;

public class SpawnBoss : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private int bossHealth = 500;
    [SerializeField] private int bossDamage = 75;

    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject Dialogue;

    private EnemyHandler bossHandler;   

    private bool spawned = false;

    private void OnEnable()
    {
        SpawnCurrentBoss();
    }

    private void Update()
    {
        if (bossHandler == null)
        {
            Debug.LogWarning("Boss handler not found. Boss may not have spawned correctly.");
            return;
        }
        if(bossHandler.GetCurrentHealth() <= 0)
        {
            Debug.Log("Boss defeated!");
            Dialogue.SetActive(true);
            portal.SetActive(true);
            gameObject.SetActive(true);
        }
    }

    private void SpawnCurrentBoss()
    {
        if (spawned) return;


        Vector3 pos = spawnArea.GetComponent<BoxCollider>().bounds.center;

        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            Vector3 spawnPos = hit.position;

            GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

            bossHandler = boss.GetComponent<EnemyHandler>();
            bossHandler.SetCurrentHealth(bossHealth);
            bossHandler.SetMaxHealth(bossHealth);
            bossHandler.SetDamage(bossDamage);
            bossHandler.SetMoneyDrop(1000);

            spawned = true;
            Debug.Log("Boss spawned at: " + spawnPos);
            return;
        }


        Debug.LogWarning("Failed to find valid boss spawn point.");
        gameObject.SetActive(false);
    }
}
