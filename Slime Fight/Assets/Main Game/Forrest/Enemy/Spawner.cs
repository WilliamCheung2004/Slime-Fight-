using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;

public class Wave
{
    public int waveNumber;
    public int spawnCount;
    public float spawnInterval;
    public int spawnedEnemies = 0;
    public float spawnTimer = 0f;
    public float waveInterval = 5f;
    public float waveTimer = 0f;
    public int enemyHealth;
    public int enemyDamage;
    public int enemyMoney;
    public List<GameObject> spawnedEnemiesList = new List<GameObject>();
}

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemies;
    public BoxCollider[] enemySpawns;

    private int startingHealth;
    private float healthMultiplier;

    private int startingDamage;
    private float damageMultiplier;

    private int startingMoney;
    private float moneyMultiplier;

    [SerializeField] private EnemyHandler enemy;
    private List<Wave> waves = new List<Wave>();
    private string currentScene;

    [SerializeField] private PlayerResource playerResource;

    [Header("Wave Settings")]
    public bool interacted = false;
    //TextMeshProUGUI waveText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemiesLeftText;
    private bool waveTextVisible = false;
    private int currentWaveIndex = 0;
    private bool waveActive = false;
    private int enemiesKilledThisWave = 0;
    [SerializeField] private int totalWaves = 3;

    [Header("Exit")]
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject exitDialogue;

    public bool canShowEnemiesLeft = true;
    public bool activatePortal = true;
    public bool StartBoss = false;
    public int spawnCount;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Forrest")
        {
            startingHealth = 1;
            healthMultiplier = 1.5f;
            startingDamage = 1;
            damageMultiplier = 1.5f;
            startingMoney = 5;
        }
        else if (currentScene == "Swamp")
        {
            startingHealth = 10;
            healthMultiplier = 1.2f;
            startingDamage = 10;
            damageMultiplier = 1.25f;
            startingMoney = 30;
        }
        else if (currentScene == "Desert")
        {
            startingHealth = 50;
            healthMultiplier = 1.25f;
            startingDamage = 40;
            damageMultiplier = 1.2f;
            startingMoney = 100;
        }
        // Create waves
        for (int i = 0; i < totalWaves; i++)
        {
            if (i == 0 && currentScene == "Forrest")
            {
                waves.Add(new Wave
                {
                    waveNumber = 1,
                    spawnCount = 1,
                    spawnInterval = 5f,
                    enemyHealth = startingHealth,
                    enemyDamage = startingDamage,
                    enemyMoney = startingMoney
                });
            }
            else
            {
                if(currentScene == "Forrest")
                {
                    spawnCount = 5 * i;
                }
                else if (currentScene == "Swamp")
                {
                    spawnCount = Mathf.RoundToInt(5 + i * 1.25f);
                }
                else if (currentScene == "Desert")
                {
                    spawnCount = Mathf.RoundToInt(5 + i * 1.5f);
                }
                waves.Add(new Wave
                {
                    waveNumber = spawnCount,
                    spawnCount =  Mathf.RoundToInt(5 + i * 1.5f), 
                    spawnInterval = Mathf.Max(0.5f, 5 - i * 0.5f),
                    enemyHealth = Mathf.RoundToInt(waves[i - 1].enemyHealth + i * 2),
                    enemyDamage = Mathf.RoundToInt(waves[i - 1].enemyDamage + i * 1),
                    enemyMoney = startingMoney * (i + 1)
                });
            }
        }
    }


    void Update()
    {
        if (currentWaveIndex >= waves.Count)
        {
            return;
        }

        Wave wave = waves[currentWaveIndex];

        if (interacted)
        {
            StartWave();
            interacted = false;
        }

        if (waveActive && currentWaveIndex < waves.Count)
        {
            HandleWave(waves[currentWaveIndex]);
        }

    }

    private void StartWave()
    {
        //currentWaveIndex = 0;
        waveActive = true;
        waveText.text = "Wave (" + waves[currentWaveIndex].waveNumber + " / " + totalWaves + ")";
        ShowWaveText();
    }
    public void EnemysKilled()
    {
        if (currentWaveIndex >= waves.Count) return;
        if (playerResource.GetCurrentHealth() < 0)
        {
            enemiesLeftText.text = "";
            return;
        }

        enemiesKilledThisWave++;
        int remaining = waves[currentWaveIndex].spawnCount - enemiesKilledThisWave;

        if (canShowEnemiesLeft && playerResource.GetCurrentHealth() > 0)
        {
            enemiesLeftText.text = "Enemies Left: " + Mathf.Max(0, remaining);
            enemiesLeftText.gameObject.SetActive(true);
        }
    }

    private void HandleWave(Wave wave)
    {

        if (wave.spawnedEnemies < wave.spawnCount)
        {
            wave.spawnTimer += Time.deltaTime;
            if (wave.spawnTimer >= wave.spawnInterval)
            {
                SpawnEnemy();
                wave.spawnedEnemies++;
                wave.spawnTimer = 0f;
            }
        }
        wave.spawnedEnemiesList.RemoveAll(e => e == null);

        if (wave.spawnedEnemies == wave.spawnCount && wave.spawnedEnemiesList.Count == 0)
        {
            currentWaveIndex++;

            if (currentWaveIndex < waves.Count)
            {
                enemiesKilledThisWave = 0;
                enemiesLeftText.text = "";
                Debug.Log($"Starting Wave {waves[currentWaveIndex].waveNumber}");
                if (playerResource.GetCurrentHealth() > 0)
                {
                    waveText.text = "Wave (" + waves[currentWaveIndex].waveNumber + " / " + totalWaves + ")";
                }
                else
                {
                    waveText.text = ""; 
                }
                    waves[currentWaveIndex].spawnTimer = 0f;
                waves[currentWaveIndex].spawnedEnemies = 0;
                ShowWaveText();
            }
            else
            {
                ShowWaveText();
                waveActive = false;
                enemiesLeftText.text = "";
                waveText.text = "Waves Completed!";
                StartCoroutine(WaitSeconds(3f));
                StartBoss = true;
            }
        }
    }

    private void SpawnEnemy()
    {
        int enemyIndex = Random.Range(0, enemies.Length);
        int spawnIndex = Random.Range(0, enemySpawns.Length);
        BoxCollider spawnArea = enemySpawns[spawnIndex];

        Vector3 spawnPos = new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            spawnArea.transform.position.y,
            Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 2f, NavMesh.AllAreas))
            spawnPos = hit.position;

        Debug.Log($"Spawning enemy with name {enemies[enemyIndex].name} at position {spawnPos}");

        GameObject enemyInstance = Instantiate(enemies[enemyIndex], spawnPos, Quaternion.identity);

        EnemyHandler handler = enemyInstance.GetComponent<EnemyHandler>();

        handler.SetMaxHealth(waves[currentWaveIndex].enemyHealth);
        handler.SetDamage(waves[currentWaveIndex].enemyDamage);
        handler.SetMoneyDrop(waves[currentWaveIndex].enemyMoney);

        if (enemyInstance.TryGetComponent<EnemyAI>(out var ai))
        {
            ai.originalPos = spawnPos;
        }
        else if (enemyInstance.TryGetComponent<AI>(out var otherAi))
        {
            otherAi.originalPos = spawnPos;
        }
        else
        {
            Debug.LogWarning("No AI script found on enemyInstance");
        }

        waves[currentWaveIndex].spawnedEnemiesList.Add(enemyInstance);
    }


    private void ShowWaveText()
    {
        StartCoroutine(ShowSeconds(3f));
    }

    IEnumerator ShowSeconds(float seconds)
    {
        canShowEnemiesLeft = false;
        waveTextVisible = true;
        waveText.gameObject.SetActive(true);

        yield return new WaitForSeconds(seconds);

        if (currentWaveIndex >= waves.Count)
            yield break;

        Wave wave = waves[currentWaveIndex];
        canShowEnemiesLeft = true;
        waveTextVisible = false;
        enemiesLeftText.gameObject.SetActive(true);
        waveText.gameObject.SetActive(false);
        enemiesLeftText.text = "Enemies Left: " + wave.spawnCount;
        wave.spawnedEnemiesList.RemoveAll(e => e == null);
    }


    IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        waveText.text = "";
        if (activatePortal)
        {
            portal.SetActive(true);
        }
    }

    IEnumerator turnOffRemain(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        enemiesLeftText.gameObject.SetActive(false);
    }
}