using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    private int currentScene;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Transform playerPos;
    [SerializeField] private SaveManager save;

    [SerializeField] private GameObject playerHealthUI;
    [SerializeField] private GameObject playerMoneyUI;
    [SerializeField] private StateManage stateManage;
    [SerializeField] private PlayerResource playerResource;
    private GameObject player;
    private Transform playerTransform;

    private IEnumerator Start()
    {
        save.CheckSave();
        while (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                playerTransform = p.transform;

            yield return null;
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 1f;
    }

    public void Continue()
    {
        stateManage.paused = false;
        playerHealthUI.SetActive(true);
        playerMoneyUI.SetActive(true);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Restart()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    public void Home()
    {
        if (playerResource.GetCurrentHealth() != 0)
        {
            save.currentData.playerCurrentHealth = playerResource.GetCurrentHealth();
            save.currentData.playerMaxHealth = playerResource.GetMaxHealth();
            save.currentData.playerDamage = playerResource.GetDamage();
            save.currentData.playerMoney = playerResource.GetMoney();
            save.currentData.playerPosition = playerTransform.position;
            save.SaveToFile();
        }
        else
        {
            save.CreateNewSave();
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ShowSettings(GameObject settingsPanel)
    {
        if (stateManage.displayedGameOver)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings(GameObject settingsPanel)
    {
        if (stateManage.displayedGameOver)
        {
            settingsPanel.SetActive(false);
        }
        else
        {
            settingsPanel.SetActive(false);
        }
    }
}
