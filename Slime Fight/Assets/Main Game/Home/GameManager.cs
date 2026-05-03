using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine.UI;
using System;
using System.Collections;
public class GameManager : MonoBehaviour
{

    [Header("Disable Objects")]
    [SerializeField] private TMPro.TextMeshProUGUI startText;
    [SerializeField] private TMPro.TextMeshProUGUI title;
    [SerializeField] private GameObject slime;

    [Header("Game Load/New")]
    [SerializeField] private GameObject GamemodeButton;
    [SerializeField] private Button LoadGameButton;

    [Header("Selection Stuff")]
    [SerializeField] private GameObject NewSelection;
    [SerializeField] private GameObject LoadSelection;
    [SerializeField] private Image targetImage;

    [Header("Load Game Text")]
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private TMPro.TextMeshProUGUI sceneText;

    [Header("Load Game Images")]
    [SerializeField] private Sprite forestSprite;
    [SerializeField] private Sprite swampSprite;
    [SerializeField] private Sprite desertSprite;

    [Header("Save/Load Stuff")]
    private string saveFilePath;
    private string SelectedPlayerType;

    [Header("Overwrite Warning")]
    [SerializeField] private GameObject overwriteWarning;

    [Header("Save")]
    [SerializeField] private SaveManager save;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting the game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetPlayerType(string playerType)
    {
        save.CreateNewSave();
        save.currentData.playerType = playerType;
        if(playerType == "Sword")
        {
            save.currentData.playerMaxHealth = 100;
            save.currentData.playerCurrentHealth = 100;
        }
        else if (playerType == "Bow")
        {
            save.currentData.playerMaxHealth = 70;
            save.currentData.playerCurrentHealth = 70;
        }

        //if player pref
        if (PlayerPrefs.HasKey("Gems") && PlayerPrefs.GetInt("Gems") > 0)
        {
            save.currentData.gems = PlayerPrefs.GetInt("Gems");
            PlayerPrefs.DeleteKey("Gems");
        }
        save.SaveToFile();
    }

    public void LoadScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }

    public void ShowGameOptions()
    {
        GamemodeButton.SetActive(true);
        bool saveExists = save.CheckSave();
        if (!saveExists ||
            (saveExists &&
            (save.currentData.currentScene == "Town" ||
             save.currentData.currentScene == "Home") &&
             !save.currentData.initialised))
        {
            LoadGameButton.interactable = false;
        }
        else if (saveExists &&
                 save.currentData.initialised &&
                 save.currentData.currentScene != "Town" ||
                 save.currentData.currentScene != "Home")
        {
            LoadGameButton.interactable = true;
        }
        else
        {
            LoadGameButton.interactable = false;
        }

        startText.gameObject.SetActive(false);
        slime.gameObject.SetActive(false);
    }

    public void TurnOffWarning(bool accept)
    {
        if (!accept)
        {
            SceneManager.LoadScene(0);
        }
        overwriteWarning.SetActive(false);
    }

    public void NewGameSelect()
    {
        if (save.CheckSave())
        {
            overwriteWarning.SetActive(true);
        }
        title.gameObject.SetActive(false);
        NewSelection.SetActive(true);
        GamemodeButton.SetActive(false);
    }

    public void LoadGameSelect()
    {
        GamemodeButton.SetActive(false);
        LoadSelection.SetActive(true);
        healthText.text = save.currentData.playerCurrentHealth.ToString() + "/" + save.currentData.playerMaxHealth.ToString();
        sceneText.text = save.currentData.currentScene.ToString();

        if(save.currentData.currentScene == "Forrest")
        {
            targetImage.sprite = forestSprite;
        }
        else if (save.currentData.currentScene == "Swamp")
        {
            targetImage.sprite = swampSprite;
        }
        else if (save.currentData.currentScene == "Desert")
        {
            targetImage.sprite = desertSprite;
        }
    }

    public void LoadSave()
    {
        SceneManager.LoadScene(save.currentData.currentScene);
    }

    public void SaveGameSelect()
    {
        //if (File.Exists(saveFilePath))
        //{
        //    Debug.Log("Save file found at " + saveFilePath);
        //    SaveData newSaveData = new SaveData
        //    {
        //        playerType = SelectedPlayerType
        //    };
        //    string json = JsonUtility.ToJson(newSaveData, true);
        //    File.WriteAllText(saveFilePath, json);
        //}
        //else
        //{
        //    Debug.Log("No save file found creating new one at " + saveFilePath);
        //    SaveData newSaveData = new SaveData
        //    {
        //        playerType = SelectedPlayerType
        //    };
        //    string json = JsonUtility.ToJson(newSaveData, true);
        //    File.WriteAllText(saveFilePath, json);
        //}
    }

}
