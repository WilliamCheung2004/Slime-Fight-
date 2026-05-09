using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerResource : MonoBehaviour
{
    [Header("Health Settings")]
    private int maxHealth;
    [SerializeField] private int currentHealth;
    private int defaultBowHealth = 70;
    private int defaultSwordHealth = 100;
    private int defaultBowDamage = 2;
    private int defaultSwordDamage = 1;

    [Header("Damage Settings")]
    private int damage = 1;

    [Header("Money Settings")]
    [SerializeField] TMP_Text storeMoneyText;
    [SerializeField] TMP_Text gameMoneyText;
    [SerializeField] private int money = 0;

    [Header("UI References")]
    public Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("Save")]
    [SerializeField] private SaveManager save;

    private GameObject player;
    string characterType;

    private void Awake()
    {
        save.CheckSave();
    }

    void Start()
    {

        if (save.currentData.playerMaxHealth > 0)
        {
            maxHealth = save.currentData.playerMaxHealth;
        }

        if (save.currentData.playerCurrentHealth > 0)
        {
            currentHealth = save.currentData.playerCurrentHealth;
        }

        else
        {
            if (save.currentData.playerType == "Sword")
            {
                maxHealth = defaultSwordHealth;
                damage = defaultSwordDamage;
            }
            else if (save.currentData.playerType == "Bow")
            {
                maxHealth = defaultBowHealth;
                damage = defaultBowDamage;
            }
            else
            {
                Debug.LogWarning("Error occured at start in PlayerResource");
            }
        }

        if (save.currentData.playerMoney > 0)
        {
            Debug.Log("Money loaded: " + save.currentData.playerMoney);
            money = save.currentData.playerMoney;
        }
        else
        {
            Debug.Log("No money found in save, defaulting to 0");
            money = 0;
        }

        SetupSlider();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        long CurrentHealthValue = currentHealth;
        string HealthValue = FormatHealth(CurrentHealthValue);
        long maxHealthValue = maxHealth;
        string MaxHealthValue = FormatHealth(maxHealthValue);
        healthSlider.value = currentHealth;
        healthText.text = HealthValue + "/" + MaxHealthValue;

        UpdateMoneyText();
    }

    void SetupSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
        }
    }

    //void TestDamageHeal()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        currentHealth -= testDamage;
    //        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    //    }

    //    if (Input.GetKeyDown(KeyCode.X))
    //    {
    //        currentHealth += testDamage;
    //        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    //    }
    //}

    public void UpdateMoneyText()
    {
        long currentMoney = money;
        gameMoneyText.text = FormatMoney(currentMoney);
        storeMoneyText.text = FormatMoney(currentMoney);
    }

    public static string FormatMoney(long value)
    {
        if (value >= 1000000000000)
            return (value / 1000000000000f).ToString("0.#") + "T";

        if (value >= 1000000000)
            return (value / 1000000000f).ToString("0.#") + "B";

        if (value >= 1000000)
            return (value / 1000000f).ToString("0.#") + "M";

        if (value >= 1000)
            return (value / 1000f).ToString("0.#") + "K";

        return value.ToString();
    }

    public static string FormatHealth(long value)
    {
        if (value >= 1000000000000)
            return (value / 1000000000000f).ToString("0.#") + "T";

        if (value >= 1000000000)
            return (value / 1000000000f).ToString("0.#") + "B";

        if (value >= 1000000)
            return (value / 1000000f).ToString("0.#") + "M";

        if (value >= 1000)
            return (value / 1000f).ToString("0.#") + "K";

        return value.ToString();
    }

    public int GetMoney()
    {
        return money;
    }

    public void SetMoney(int amount)
    {
        money = amount;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public int TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        return currentHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetCurrentHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        SetupSlider();
    }


}
