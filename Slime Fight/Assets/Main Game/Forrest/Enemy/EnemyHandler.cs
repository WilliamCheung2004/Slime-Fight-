using UnityEngine;

public class EnemyHandler : MonoBehaviour
{

    [Header("Enemy Settings")]
    private int maxHealth;
    private int currentHealth;
    private int damage;

    [Header("Enemy Health UI")]
    [SerializeField] private TMPro.TMP_Text healthText;
    [SerializeField] private UnityEngine.UI.Slider healthSlider;

    [Header("Enemy Drops")]
    private int moneyDrop;


    private void Start()
    {
    }
    void Update()
    {
        if (maxHealth > 0)
        {
            SetHealthText();
            UpdateSlider();
        }
        else
        {
            return;
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetCurrentHealth(int newHealth)
    {
        currentHealth = newHealth;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        SetupSlider();
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    private void SetHealthText()
    {
        healthText.text = currentHealth + "/" + maxHealth;
        Debug.Log("Health Text Updated: " + healthText.text);
    }

    void SetupSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
        }
    }

    void UpdateSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            healthSlider.maxValue = maxHealth; 
            healthSlider.minValue = 0;
        }
    }

    public int GetMoneyDrop()
    {
        return moneyDrop;
    }

    public void SetMoneyDrop(int newMoneyDrop)
    {
        moneyDrop = newMoneyDrop;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}
