using System.Collections;
using UnityEngine;
using EasyTextEffects;

public class StopTextEffectAfterTime : MonoBehaviour
{
    [Tooltip("How many seconds until the effect stops")]
    public float duration = 2f;

    private TextEffect textEffect;

    private void Awake()
    {
        textEffect = GetComponent<TextEffect>();
    }

    private void Start()
    {
        StartCoroutine(StopAfter(duration));
    }

    private IEnumerator StopAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (textEffect != null)
        {
            textEffect.StopAllEffects();       
        }
    }
}