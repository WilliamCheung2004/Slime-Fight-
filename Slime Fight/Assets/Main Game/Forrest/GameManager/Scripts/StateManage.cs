using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class StateManage : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private PlayerResource player;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject Upgrades;
    [SerializeField] private GameObject Skins;

    [SerializeField] private GameObject playerHealthUI;
    [SerializeField] private GameObject playerMoneyUI;

    public bool paused = false;
    public bool shopOpen = false;

    public bool displayedGameOver = false;

    void Start()
    {
        PlayerPrefs.SetInt("DialogueStatus", 1);
        Cursor.lockState = CursorLockMode.Locked;
        SoundManager.PlayLoopSound(SoundType.BACKGROUND, 0.08f);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (player.GetCurrentHealth() <= 0 && !displayedGameOver)
        {
            GameOver();
            displayedGameOver = true;
        }

        if (Input.GetKeyDown(KeyCode.B) && !paused)
        {
            DisplayShop();
        }
    }

    private void TogglePause()
    {
        paused = !paused;

        if (paused)
        {
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            playerHealthUI.SetActive(false);
            playerMoneyUI.SetActive(false);
            shopUI.SetActive(false);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            playerHealthUI.SetActive(true);
            playerMoneyUI.SetActive(true);
        }
    }

    private void GameOver()
    {
        paused = true;
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        playerHealthUI.SetActive(false);
        playerMoneyUI.SetActive(false);
        shopUI.SetActive(false);
    }

    private void DisplayShop()
    {
        if (paused) return;
        shopOpen = !shopOpen;
        if (shopOpen)
        {
            Time.timeScale = 0f;
            shopUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            shopUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ShowUpgrades()
    {
        Upgrades.SetActive(true);
        Skins.SetActive(false);
    }

    public void ShowSkins()
    {
        Skins.SetActive(true);
        Upgrades.SetActive(false);
    }
}
