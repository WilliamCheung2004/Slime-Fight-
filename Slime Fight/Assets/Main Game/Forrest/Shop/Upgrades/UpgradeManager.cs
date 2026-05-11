using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class UpgradeUI
{
    public string upgradeName;
    public TMP_Text costText;
    public int cost;
    public Button upgradeButton;
    public System.Action onPurchase;
    public bool isMaxed = false;
    public float costMultiplier;
    public ToolTip toolTip;

    public void UpdateCostText()
    {
        if (costText != null)
        {
            costText.text = cost.ToString();
        }
    }
}

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private StateManage state;
    [SerializeField] private UpgradeUI[] upgrades;
    [SerializeField] private PlayerResource playerResource;

    private bool canRegen = false;
    private float regenTimer = 0f;
    [SerializeField] private float regenInterval = 1f;
    private int regenAmount = 0;

    private int damageIncrease = 1;

    private float AttackSpeed = 0.05f;
    private Animator weapon;

    private float Speed = 0.1f;
    private PlayerMotor playerMotor;

    private float Efficiency = 0f;

    [SerializeField] private SaveManager save;
    private SaveManager saveManager;

    private GameObject weaponObj = null;
    private BowController bowController;

    void Start()
    {
        saveManager = save.GetComponent<SaveManager>();
        saveManager.CheckSave();

        StartCoroutine(SetupWeapon());

        playerMotor = FindObjectOfType<PlayerMotor>();

        foreach (var ui in upgrades)
        {
            Upgrade saved = saveManager.currentData.upgrades.GetUpgrade(ui.upgradeName);

            if (saved == null)
            {
                Debug.LogWarning("Upgrade not found in save: " + ui.upgradeName);
                continue;
            }

            if (saved.cost <= 0)
            {
                saved.cost = saved.baseCost;
                saveManager.currentData.upgrades.GetUpgrade(ui.upgradeName).cost = saved.cost;
            }
            else
            {
                saved.cost = Mathf.RoundToInt(saved.cost);
            }

            ui.cost = saved.cost;
            ui.UpdateCostText();


            switch (ui.upgradeName)
            {
                case "Health":
                    if (saved.currentValue > 0)
                    {
                        upgrades[0].toolTip.line2 = "Current max health: " + playerResource.GetMaxHealth() + " HP";
                    }
                    break;

                case "Regen":
                    if (saved.currentValue > 0)
                    {
                        regenAmount = (int)saved.currentValue;
                        canRegen = true;
                        upgrades[1].toolTip.line2 = "Current regen amount: " + regenAmount + " /S";
                    }
                    break;

                case "Damage":
                    if (saveManager.currentData.playerDamage > 0)
                    {
                        playerResource.SetDamage(saveManager.currentData.playerDamage);
                        upgrades[2].toolTip.line2 = "Current damage: " + playerResource.GetDamage() + " DMG";
                    }
                    break;

                case "AttackSpeed":
                    if (saved.currentValue > 0 && weapon != null)
                    {
                        weapon.speed = saved.currentValue;
                        upgrades[3].toolTip.line2 = "Current attack speed: " + weapon.speed + "/HPS";
                    }
                    break;

                case "Speed":
                    if (saved.currentValue > 0)
                    {
                        playerMotor.speed = saved.currentValue;
                        upgrades[4].toolTip.line2 = "Current speed: " + playerMotor.speed + "/ms";
                    }
                    else
                    {
                        playerMotor.speed = playerMotor.defaultSpeed;
                    }
                        break;

                case "MoneyEfficiency":
                    if (saved.currentValue > 0)
                    {
                        Efficiency = saved.currentValue;
                        upgrades[5].toolTip.line2 = "Current money efficiency: " + Mathf.RoundToInt(Efficiency * 100) + "%";
                    }
                    break;
            }
        }

        saveManager.SaveToFile();
        upgrades[0].costMultiplier = 1.5f; 
        upgrades[1].costMultiplier = 1.5f; 
        upgrades[2].costMultiplier = 1.25f; 
        upgrades[3].costMultiplier = 1.2f;
        upgrades[4].costMultiplier = 1.2f; 
        upgrades[5].costMultiplier = 1.5f;
    }


    void Update()
    {
        CanBuy();

        if (canRegen)
        {
            Regen();
        }

        upgrades[0].onPurchase = () => IncreaseHealth();
        upgrades[1].onPurchase = () =>
        {
            regenAmount += 1;
            saveManager.currentData.upgrades.GetUpgrade(upgrades[1].upgradeName).currentValue = regenAmount;
            if (regenAmount >= 50)
            {
                upgrades[1].upgradeButton.interactable = false;
                upgrades[1].upgradeButton.GetComponentInChildren<TMP_Text>().text = "Maxed";
                upgrades[1].isMaxed = true;
            }

            Regen();
        };
        upgrades[2].onPurchase = () => IncreaseDamage();
        upgrades[3].onPurchase = () => IncreaseAttackSpeed();
        upgrades[4].onPurchase = () => IncreaseSpeed();
        upgrades[5].onPurchase = () => MoneyEfficiency();
    }

    private IEnumerator SetupWeapon()
    {
        string playerType = saveManager.currentData.playerType;

        string tagToFind = "";

        if (playerType == "Sword")
            tagToFind = "Sword";
        else if (playerType == "Bow")
            tagToFind = "Bow";
        else
            yield break;

        while (weaponObj == null)
        {
            weaponObj = GameObject.FindGameObjectWithTag(tagToFind);
            yield return null;
        }

        weapon = weaponObj.GetComponent<Animator>();
        if (weapon)
        {
            Debug.Log("Weapon found" + weaponObj.name);
        }
    }

    private void CanBuy()
    {
        foreach (var u in upgrades)
        {
            if (playerResource.GetMoney() >= u.cost && !u.isMaxed)
            {
                u.upgradeButton.interactable = true;
            }
            else
            {
                u.upgradeButton.interactable = false;
            }
        }
    }

    public void Purchase(string upgradeName)
    {
        if (!state.shopOpen) return;

        foreach (var u in upgrades)
        {
            if (u.upgradeName != upgradeName) continue;

            if (playerResource.GetMoney() >= u.cost)
            {
                playerResource.SetMoney(playerResource.GetMoney() - u.cost);
                playerResource.UpdateMoneyText();
                u.onPurchase?.Invoke();

                Upgrade costUpgrade = saveManager.currentData.upgrades.GetUpgrade(u.upgradeName);
                if (costUpgrade == null)
                {
                    Debug.LogWarning("Upgrade not found in save: " + u.upgradeName);
                    continue;
                }
       
                u.cost = Mathf.RoundToInt(u.cost * u.costMultiplier);
                costUpgrade.cost = u.cost;
                save.currentData.playerMoney = playerResource.GetMoney();
                u.UpdateCostText();
                saveManager.SaveToFile();
            }

            break;
        }
    }

    private void IncreaseHealth()
    {
        Upgrade healthUpgrade = saveManager.currentData.upgrades.GetUpgrade(upgrades[0].upgradeName);

        playerResource.SetMaxHealth(playerResource.GetMaxHealth() + 50);
        playerResource.healthSlider.maxValue = playerResource.GetMaxHealth();
        playerResource.UpdateMoneyText();
        healthUpgrade.currentValue = playerResource.GetMaxHealth();
        saveManager.currentData.playerMaxHealth = playerResource.GetMaxHealth();    
        if (playerResource.GetMaxHealth() >= 1000)
        {
            upgrades[0].upgradeButton.interactable = false;
            upgrades[0].upgradeButton.GetComponentInChildren<TMP_Text>().text = "Maxed";
            upgrades[0].isMaxed = true;
        }

        upgrades[0].toolTip.line2 = "Current max health: " + playerResource.GetMaxHealth() + " HP";
    }

    private void Regen()
    {
        Upgrade regenUpgrade = saveManager.currentData.upgrades.GetUpgrade(upgrades[1].upgradeName);

        if (!canRegen)
            canRegen = true;

        if (regenTimer < regenInterval)
        {
            regenTimer += Time.deltaTime;
            return;
        }

        playerResource.SetCurrentHealth(playerResource.GetCurrentHealth() + regenAmount);
        playerResource.SetCurrentHealth(Mathf.Clamp(playerResource.GetCurrentHealth(), 0, playerResource.GetMaxHealth()));

        regenTimer = 0f;

        regenUpgrade.currentValue = regenAmount;
        upgrades[1].toolTip.line2 = "Current regen amount: " + regenAmount + " /S";
    }


    private void IncreaseDamage()
    {
        Upgrade damageUpgrade = saveManager.currentData.upgrades.GetUpgrade(upgrades[2].upgradeName);

        playerResource.SetDamage(playerResource.GetDamage() + damageIncrease);

        damageUpgrade.currentValue = playerResource.GetDamage();
        upgrades[2].toolTip.line2 = "Current damage: " + playerResource.GetDamage() + " DMG";
    }

    private void IncreaseAttackSpeed()
    {
        Upgrade attackSpeedUpgrade = saveManager.currentData.upgrades.GetUpgrade(upgrades[3].upgradeName);
        if (weaponObj && save.currentData.playerType == "Bow")
        {
            bowController = weaponObj.GetComponent<BowController>();
            Debug.Log("Bow cooldown before: " + bowController.arrowCooldown);
            bowController.arrowCooldown -= AttackSpeed;
            bowController.arrowCooldown = Mathf.Clamp(bowController.arrowCooldown - AttackSpeed, 0.1f, float.MaxValue);
            Debug.Log("Bow cooldown after: " + bowController.arrowCooldown);
        }

        weapon.speed = Mathf.Clamp(weapon.speed + AttackSpeed, 1f, 2f);

        if (weapon.speed >= 1.99f)
        {
            upgrades[3].upgradeButton.interactable = false;
            upgrades[3].upgradeButton.GetComponentInChildren<TMP_Text>().text = "Maxed";
            upgrades[3].isMaxed = true;
        }
        attackSpeedUpgrade.currentValue = weapon.speed;
        upgrades[3].toolTip.line2 = "Current attack speed: " + weapon.speed + "/HPS";
    }

    private void IncreaseSpeed()
    {
        Upgrade attackSpeedUpgrade = saveManager.currentData.upgrades.GetUpgrade(upgrades[3].upgradeName);

        if (weaponObj && saveManager.currentData.playerType == "Bow")
        {
            bowController = weaponObj.GetComponent<BowController>();

            if (bowController != null)
            {
                bowController.arrowCooldown = Mathf.Clamp(
                    bowController.arrowCooldown - AttackSpeed,
                    0.1f,
                    float.MaxValue
                );
            }
        }

        weapon.speed = Mathf.Clamp(weapon.speed + AttackSpeed, 1f, 2f);

        if (weapon.speed >= 1.99f)
        {
            upgrades[3].upgradeButton.interactable = false;
            upgrades[3].upgradeButton.GetComponentInChildren<TMP_Text>().text = "Maxed";
            upgrades[3].isMaxed = true;
        }

        attackSpeedUpgrade.currentValue = weapon.speed;
        upgrades[3].toolTip.line2 = "Current attack speed: " + weapon.speed + "/HPS";
    }


    public float GetMoneyEfficiency()
    {
        return Efficiency;
    }

    private void MoneyEfficiency()
    {
        Upgrade moneyEfficiencyUpgrade = saveManager.currentData.upgrades.GetUpgrade(upgrades[5].upgradeName);

        Efficiency = Mathf.Clamp(Efficiency + 0.05f, 0, 1);

        if (Efficiency >= 0.99f)
        {
            upgrades[5].upgradeButton.interactable = false;
            upgrades[5].upgradeButton.GetComponentInChildren<TMP_Text>().text = "Maxed";
            upgrades[5].isMaxed = true;
        }

        moneyEfficiencyUpgrade.currentValue = Efficiency;

        upgrades[5].toolTip.line2 = "Current money efficiency: " + Mathf.RoundToInt(Efficiency * 100) + "%";
    }
}