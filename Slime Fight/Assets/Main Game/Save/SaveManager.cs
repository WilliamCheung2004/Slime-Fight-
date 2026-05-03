using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public string saveFilePath;
    public SaveData currentData;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public bool CheckSave()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No save file found");
            return false;
        }
        else
        {
            string json = File.ReadAllText(saveFilePath);
            currentData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Save Loaded");
            return true;
        }
    }

    private UpgradeData CreateDefaultUpgrades()
    {
        UpgradeData data = new UpgradeData();
        data.upgradeList = new List<Upgrade>()
    {
        new Upgrade { name = "Health", baseCost = 5, cost = 0, currentValue = 0 },
        new Upgrade { name = "Regen", baseCost = 20, cost = 0, currentValue = 0 },
        new Upgrade { name = "Damage", baseCost = 20, cost = 0, currentValue = 0 },
        new Upgrade { name = "AttackSpeed", baseCost = 25, cost = 0, currentValue = 0 },
        new Upgrade { name = "Speed", baseCost = 50, cost = 0, currentValue = 0 },
        new Upgrade { name = "MoneyEfficiency", baseCost = 100, cost = 0, currentValue = 0 }
    };

        return data;
    }


    private SaveData CreateDefaultSave()
    {
        return new SaveData
        {
            playerType = "None",
            currentScene = SceneManager.GetActiveScene().name,
            currentWaveIndex = 0,
            enemiesSpawned = 0,
            playerMoney = 0,
            playerCurrentHealth = 0,
            playerMaxHealth =0,
            playerDamage = 1,
            playerPosition = Vector3.zero,
            initialised = false,
            upgrades = CreateDefaultUpgrades(),
            gems = 0,
        };
    }

    public void SaveToFile()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(saveFilePath, json);
    }

    public void CreateNewSave()
    {
        currentData = CreateDefaultSave();
        SaveToFile();
    }

    public void SetUpgrade(string fieldName, float value)
    {
        var field = typeof(SaveData).GetField(fieldName);
        if (field == null)
        {
            Debug.LogError($"Field '{fieldName}' not found in SaveData");
            return;
        }

        field.SetValue(currentData, value);
        SaveToFile();
    }

    public void AddInteracted(string obj)
    {
        if (!currentData.interacted.Contains(obj))
        {
            currentData.interacted.Add(obj);
            SaveToFile();
        }
    }

    public bool HasInteracted(string obj)
    {
        return currentData.interacted.Contains(obj);
    }

}