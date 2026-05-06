using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class DailyReward : MonoBehaviour
{
    [SerializeField] private Button rewardButton;
    [SerializeField] private TMPro.TMP_Text countdownText;
    [SerializeField] private GameObject reward;
    [SerializeField] private SaveManager save;

    private const string Claim = "LastClaimTime";
    private TimeSpan cooldown = TimeSpan.FromHours(24);
    public bool ignoreCooldown = false;

    private bool saveStatus;

    void Start()
    {
        saveStatus = save.CheckSave();
        rewardButton.onClick.AddListener(ClaimReward);
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void ClaimReward()
    {
        if (CanClaim())
        {
            Debug.Log("Reward claimed!");
            PlayerPrefs.SetString(Claim, DateTime.UtcNow.ToString());
            PlayerPrefs.Save();
            if (!saveStatus)
            {
                if (PlayerPrefs.HasKey("Gems"))
                {
                    int currentGems = PlayerPrefs.GetInt("Gems");
                    PlayerPrefs.SetInt("Gems", currentGems + 10);
                }
                else
                {
                    PlayerPrefs.SetInt("Gems", 10);
                }
            }
            else
            {
                save.currentData.gems += 10;
                save.SaveToFile();
            }
            UpdateUI();
        }
    }

    bool CanClaim()
    {
        if (!PlayerPrefs.HasKey(Claim))
            return true;

        if (ignoreCooldown)
            return true;

        DateTime lastClaim = DateTime.Parse(PlayerPrefs.GetString(Claim));
        return DateTime.UtcNow - lastClaim >= cooldown;
    }

    public void RewardClicked()
    {
        reward.SetActive(true);
    }

    void UpdateUI()
    {
        if (CanClaim())
        {
            countdownText.text = "Claim";
            rewardButton.interactable = true;
        }
        else
        {
            DateTime lastClaim = DateTime.Parse(PlayerPrefs.GetString(Claim));
            TimeSpan timeLeft = (lastClaim + cooldown) - DateTime.UtcNow;

            countdownText.text = FormatTime(timeLeft);
            rewardButton.interactable = false;
        }
    }

    string FormatTime(TimeSpan t)
    {
        if (t.TotalHours < 1)
            return $"{t.Minutes:D2}:{t.Seconds:D2}";
        else if (t.TotalMinutes < 1)
        {
            return $"{t.Seconds:D2}s";
        }
        else
        {
            return $"{(int)t.TotalHours}h {t.Minutes:D2}m {t.Seconds:D2}s";
        }
    }
}
