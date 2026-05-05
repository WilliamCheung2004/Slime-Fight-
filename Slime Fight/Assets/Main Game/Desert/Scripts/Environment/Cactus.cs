using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : MonoBehaviour
{
    public int damage = 1;
    public float damageInterval = 1f;
    private float timer = 0f;
    [SerializeField] private PlayerResource playerResource;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerResource != null)
            {
                timer += Time.deltaTime;

                if (timer >= damageInterval)
                {
                    playerResource.SetCurrentHealth(playerResource.GetCurrentHealth() - damage);
                    timer = 0f;
                }
            }
        }
    }
}