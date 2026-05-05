using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    private int currentScene;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Transform playerPos;
    [SerializeField] private SaveManager save;

    [SerializeField] private GameObject playerHealthUI;
    [SerializeField] private GameObject playerMoneyUI;
    [SerializeField] private StateManage stateManage;

    private void OnEnable()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        save.CheckSave();
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
        if (save.currentData.playerCurrentHealth != 0)
        {
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
