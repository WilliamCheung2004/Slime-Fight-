using UnityEngine;
using TMPro;

public class MoneyFormat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private PlayerResource playerResource;

    private void Update()
    {
        long money = playerResource.GetMoney();
        Debug.Log("Money before" + money);
        moneyText.text = FormatMoney(money);
        Debug.Log("Money After" + moneyText.text);
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
}