using UnityEngine.UI;
using UnityEngine;

public class InitialiseSound : MonoBehaviour
{
    [SerializeField] private SoundManager buttonManager;
    [SerializeField] private Slider currentSlider;

    public bool isCurrentEffectsSlider;
    public bool isCurrentBackgroundSlider; 

    void Start()
    {
        if (isCurrentBackgroundSlider)
        {
            buttonManager.InitialiseBackgroundSlider(currentSlider);
        }
        else if (isCurrentEffectsSlider)
        {
            buttonManager.InitialiseEffectsSlider(currentSlider);
        }
    }
}
