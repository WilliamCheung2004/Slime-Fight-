using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialiseImage : MonoBehaviour
{
    [SerializeField] private SaveManager save;
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite bowImage;
    [SerializeField] private Sprite swordImage;

    private void Start()
    {
        save.CheckSave();
    }
    private void OnEnable()
    {
        targetImage.sprite = save.currentData.playerType == "Sword" ? swordImage : bowImage;
    }
}
