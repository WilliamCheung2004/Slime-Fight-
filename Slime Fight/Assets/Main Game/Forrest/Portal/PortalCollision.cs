using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalCollision : MonoBehaviour
{
    [SerializeField] SaveManager save;
    private PlayerResource playerResource;
    [SerializeField] private int gemsReward;

    void Start()
    {
        playerResource = GameObject.FindGameObjectWithTag("Player Resources")?.GetComponent<PlayerResource>();
        if (playerResource == null)
        {
            Debug.LogError("PlayerResource not found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the portal!");
            save.currentData.initialised = false;
            save.currentData.playerPosition = new Vector3(0, 0, 0);
            save.currentData.playerCurrentHealth = playerResource.GetCurrentHealth();
            save.currentData.playerMaxHealth = playerResource.GetMaxHealth();
            save.currentData.playerDamage = playerResource.GetDamage();
            save.currentData.playerMoney = playerResource.GetMoney();
            save.currentData.gems += gemsReward;
            save.SaveToFile();
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex < SceneManager.sceneCountInBuildSettings - 1)
            {
                SceneManager.LoadScene(currentSceneIndex + 1);
            }
            else
            {
                Debug.Log("No more scenes to load!");
            }
        }

    }

}
