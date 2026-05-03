using UnityEngine;
using UnityEngine.UI;

public class ImageHover : MonoBehaviour
{
    public Image image;
    [SerializeField] private Color originalColor;
    [SerializeField] private Color hoverColor = Color.gray;

    void Start()
    {
        originalColor = image.color;
    }

    public void OnHoverEnter()
    {
        image.color = hoverColor;
    }

    public void OnHoverExit()
    {
        image.color = originalColor;
    }
}