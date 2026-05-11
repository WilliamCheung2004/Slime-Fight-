using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private EnemyHandler enemy;
    private GameObject player;
    private PlayerResource playerResource;
    private SaveManager save;
    public int damage = 1;
    public float damageMultiplier = 1;    

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player Resources");
        playerResource = player.GetComponent<PlayerResource>();
        save = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
    }

    private void Start()
    {
        if (save)
        {
            save.CheckSave();
            if(save.currentData.playerDamage > 0)
            {
                damage = Mathf.RoundToInt(save.currentData.playerDamage * 1.1f);
            }
        }
    }

    public void LowHit()
    {
        damageMultiplier = 0.25f;
    }

   public void MediumHit()
    {
        damageMultiplier = 0.5f;
    }

    public void HighHit()
    {
        damageMultiplier = 1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rb == null) return;

        var other = collision.collider;

        Debug.Log($"Arrow hit enemy for {Mathf.RoundToInt(damage * damageMultiplier)} damage.");

        if (other.CompareTag("Enemy"))
        {
            var enemyHandler = other.GetComponent<EnemyHandler>();
            if (enemyHandler != null)
            {
                enemyHandler.SetCurrentHealth(enemyHandler.GetCurrentHealth() - Mathf.RoundToInt(damage * damageMultiplier));
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