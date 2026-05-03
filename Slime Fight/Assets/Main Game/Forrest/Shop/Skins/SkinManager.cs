using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class PlayerSkin
{
    public string skinName;
    public int cost;
    public TMP_Text skinCostText;
    public Button button;
    public bool owned = false;
    public bool equipped = false;
}

public class SkinManager : MonoBehaviour
{
    [SerializeField] private TMP_Text currentGemsText;
    [SerializeField] private PlayerSkin[] skins;
    [SerializeField] private SaveManager save;

    private void Start()
    {
        save.CheckSave();

        currentGemsText.text = save.currentData.gems.ToString();

        //foreach (var skin in skins)
        //{
        //    if (save.currentData.equippedSkin == skin.skinName)
        //        skin.equipped = true;
        //}

        InitialiseSkins();
    }

    private void InitialiseSkins()
    {
        foreach (var skin in skins)
        {
            if (!skin.owned)
                skin.skinCostText.text = skin.cost.ToString();
            else if (skin.equipped)
                skin.skinCostText.text = "Equipped";
            else
                skin.skinCostText.text = "Apply?";

            skin.button.onClick.AddListener(() => OnSkinButtonClicked(skin));
        }
    }

    private void Update()
    {
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        foreach (var skin in skins)
        {
            if (skin.owned)
            {
                skin.button.interactable = true;
                continue;
            }

            skin.button.interactable = save.currentData.gems >= skin.cost;
        }
    }

    private void OnSkinButtonClicked(PlayerSkin skin)
    {
        if (!skin.owned)
        {
            TryPurchaseSkin(skin);
            return;
        }

        EquipSkin(skin);
    }

    private void TryPurchaseSkin(PlayerSkin skin)
    {
        if (save.currentData.gems < skin.cost)
            return;

        save.currentData.gems -= skin.cost;
        currentGemsText.text = save.currentData.gems.ToString();

        skin.owned = true;

        skin.skinCostText.text = "Apply?";

        save.SaveToFile();
    }

    private void EquipSkin(PlayerSkin skin)
    {
        foreach (var s in skins)
        {
            s.equipped = false;

            if (s.owned)
                s.skinCostText.text = "Apply?";
        }


        skin.equipped = true;
        skin.skinCostText.text = "Equipped";

        Debug.Log("Equipped skin: " + skin.skinName);
    }
}
