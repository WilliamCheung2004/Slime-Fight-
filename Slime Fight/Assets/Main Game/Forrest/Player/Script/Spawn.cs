using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Spawn : MonoBehaviour
{
    [SerializeField] private GameObject playerSwordPrefab;
    [SerializeField] private GameObject playerBowPrefab;
    [SerializeField] private GameObject playerSpawnPoint;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private SaveManager save;

    private GameObject player;
    void Start()
    {
        save.CheckSave();
        SpawnPlayer();
        gameManager.SetActive(true);
    }

    private void SpawnPlayer()
    {
        Debug.Log("Current Set:" + save.currentData.playerType);
        if (playerSpawnPoint == null)
        {
            Debug.LogError("Spawn point missing");
            return;
        }

        string type = save.currentData.playerType;
        Debug.Log(type);

        GameObject prefabToSpawn = null;

        if (type == "Sword")
            prefabToSpawn = playerSwordPrefab;
        else if (type == "Bow")
            prefabToSpawn = playerBowPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogError("Invalid player type or prefab missing");
            return;
        }

        Vector3 spawnPos = save.currentData.initialised
            ? save.currentData.playerPosition
            : playerSpawnPoint.transform.position;

        player = Instantiate(prefabToSpawn, spawnPos, playerSpawnPoint.transform.rotation);
        player.transform.SetParent(playerSpawnPoint.transform, true);

        save.currentData.playerPosition = spawnPos;
        save.currentData.currentScene = SceneManager.GetActiveScene().name;
        save.currentData.initialised = true;
        save.SaveToFile();
    }
}