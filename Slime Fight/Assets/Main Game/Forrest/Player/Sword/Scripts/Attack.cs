using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class ClickAttack : MonoBehaviour
{
    public Animator animator;

    [Header("Attack Settings")]
    private bool isAttacking = false;
    private bool hasHitEnemy = false; 

    private EnemyHandler enemy;
    private PlayerResource player;

    private void Start()
    {
        player = FindObjectsOfType <PlayerResource>()[0];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && PlayerPrefs.GetInt("DialogueStatus") == 1)
        {
            StartAttack();
        }
    }

    public void playSound()
    {
        SoundManager.PlaySound(SoundType.SWORD);
    }

    void StartAttack()
    {
        isAttacking = true;
        hasHitEnemy = false; 
        animator.SetTrigger("Attack");
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void OnTriggerStay(Collider other)
    {
   
        if (other.CompareTag("Enemy") && isAttacking && !hasHitEnemy)
        {
            Debug.Log("Hit" + other.name);
            EnemyHandler enemy = other.GetComponent<EnemyHandler>();
            if (enemy != null)
            {
                enemy.SetCurrentHealth(enemy.GetCurrentHealth() - player.GetDamage());
            }

            hasHitEnemy= true; 
        }
    }
}