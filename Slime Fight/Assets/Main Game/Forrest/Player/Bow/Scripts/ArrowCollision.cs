using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private EnemyHandler enemy;
    private GameObject player;
    private PlayerResource playerResource;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player Resources");
        playerResource = player.GetComponent<PlayerResource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Arrow collided with: " + collision.collider.name);
        if (rb == null) return;

        var other = collision.collider;

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Arrow hit an enemy!");
            var enemyHandler = other.GetComponent<EnemyHandler>();
            if (enemyHandler != null)
            {
                enemyHandler.SetCurrentHealth(enemyHandler.GetCurrentHealth() - playerResource.GetDamage());
            }
            Destroy(gameObject);
            return;
        }

        rb.isKinematic = true;
        rb.useGravity = false;
        //rb.velocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Destroy(gameObject, 5f);
    }



}