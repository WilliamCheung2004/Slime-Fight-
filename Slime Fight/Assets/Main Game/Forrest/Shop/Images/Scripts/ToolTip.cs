using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip Lines")]
    public string line1;
    public string line2;

    public void OnPointerEnter(PointerEventData eventData)
    {
        string message = line1 + "\n" + line2;
        ToolTipManager._instance.SetAndShowToolTip(message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager._instance.HideToolTip();
    }
}