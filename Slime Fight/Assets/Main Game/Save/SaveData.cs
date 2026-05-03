using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string playerType;
    public string currentScene;
    public int currentWaveIndex;
    public int enemiesSpawned;
    public int playerMoney;
    public int playerCurrentHealth;
    public int playerMaxHealth;
    public int playerDamage;
    public Vector3 playerPosition;
    public bool initialised;
    public List<string> interacted = new List<string>();
    public UpgradeData upgrades;
    public int gems;
}
[System.Serializable]
public class Upgrade
{
    public string name;
    public int cost;
    public int baseCost;
    public float currentValue;
}

[System.Serializable]
public class UpgradeData
{
    public List<Upgrade> upgradeList = new List<Upgrade>();

    public Upgrade GetUpgrade(string name)
    {
        return upgradeList.Find(u => u.name == name);
    }
}