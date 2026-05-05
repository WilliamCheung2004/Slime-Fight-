using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private PlayerResource playerResource;
    private EnemyHandler enemy;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player Resources");
        playerResource = player.GetComponent<PlayerResource>();
    }

    public void Initialise(EnemyHandler enemyHandler)
    {
        enemy = enemyHandler;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemy == null) return; 
        if (playerResource == null) return;

        if (other.gameObject.CompareTag("Player"))
        {
            playerResource.SetCurrentHealth(playerResource.GetCurrentHealth() - enemy.GetDamage());
        }
    }
}
