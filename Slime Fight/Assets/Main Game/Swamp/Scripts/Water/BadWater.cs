using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BadWater : MonoBehaviour
{
    public Color fogColor = new Color(0.0f, 0.4f, 0.7f, 1f);
    public float fogDensity = 0.05f;

    private Color defaultColor;
    private float defaultDensity;
    private FogMode defaultMode;
    private float defaultStart;
    private float defaultEnd;

    [SerializeField] private PlayerResource playerResource;
    private PlayerMotor playerMotor;
    private float damageTimer = 0f;
    private bool effectsApplied = false;

    IEnumerator Start()
    {
        while (playerMotor == null || playerResource == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerMotor = player.GetComponent<PlayerMotor>();
            }
            yield return null;
        }

        Collider c = GetComponent<Collider>();
        if (c.bounds.Contains(playerMotor.transform.position))
        {
            ApplyWaterEffects();
            effectsApplied = true;
        }
    }

    private void ApplyWaterEffects()
    {
        defaultMode = RenderSettings.fogMode;
        defaultStart = RenderSettings.fogStartDistance;
        defaultEnd = RenderSettings.fogEndDistance;
        defaultColor = RenderSettings.fogColor;
        defaultDensity = RenderSettings.fogDensity;

        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = new Color(0.05f, 0.15f, 0.08f);
        RenderSettings.fogDensity = 0.4f;

        playerMotor.speed = playerMotor.speed / 2f;
        playerMotor.gravity = -4f;
    }

    private void RestoreDefaults()
    {
        RenderSettings.fogMode = defaultMode;
        RenderSettings.fogStartDistance = defaultStart;
        RenderSettings.fogEndDistance = defaultEnd;
        RenderSettings.fogColor = defaultColor;
        RenderSettings.fogDensity = defaultDensity;

        playerMotor.speed = playerMotor.defaultSpeed;
        playerMotor.gravity = playerMotor.defaultGravity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || playerMotor == null || playerResource == null) return;
        if (!effectsApplied)
        {
            SoundManager.SelectSound(SoundType.BACKGROUND, 1, 0.5f);
            ApplyWaterEffects();
            effectsApplied = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || playerMotor == null) return;

        damageTimer += Time.deltaTime;
        if (damageTimer >= 1f)
        {
            playerResource.SetCurrentHealth(playerResource.GetCurrentHealth() - 1);
            damageTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || playerMotor == null) return;

        RestoreDefaults();
        effectsApplied = false;
        damageTimer = 0f;
    }
}
