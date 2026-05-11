using System.Collections;
using UnityEngine;
public class BowController : MonoBehaviour
{
    [Header("Animator References")]
    [SerializeField] private Animator bow;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private GameObject player;
    private PlayerResource playerResource;
    [SerializeField] private Camera playerCamera;

    [Header("Arrow Settings")]
    [SerializeField] private float arrowSpeed = 5f;
    private float originalSpeed;
    private GameObject currentArrow;
    private Rigidbody currentArrowRb;

    [Header("Arrow Cooldown")]
    public float arrowCooldown = 1f;
    private float cooldownTimer = 0f;
    private bool canCreateArrow = true;
    private bool canShootArrow = false;

    [SerializeField] private SaveManager save;

    IEnumerator Start()
    {
        while (playerResource == null)
        {
            GameObject playerResourceInstance = GameObject.FindGameObjectWithTag("Player Resources");
            if (playerResourceInstance != null)
            {
                playerResource = playerResourceInstance.GetComponent<PlayerResource>();
            }
            yield return null;
        }
    }

    void Update()
    {

        if (bow.speed > 0 && originalSpeed != bow.speed)
        {
            originalSpeed = bow.speed;
        }

        if (!canCreateArrow)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= arrowCooldown)
            {
                canCreateArrow = true;
                cooldownTimer = 0f;
            }
        }

        if (PlayerPrefs.GetInt("DialogueStatus") == 1)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (canCreateArrow && currentArrow == null)
                {
                    CreateArrow();
                    canCreateArrow = false;
                    canShootArrow = true;
                    bow.SetBool("Firing", true);

                }
                else
                {
                    canShootArrow = false;
                }
            }


            if (Input.GetMouseButtonUp(1) && canShootArrow)
            {
                canShootArrow = false;
                bow.speed = originalSpeed;
                bow.SetBool("Firing", false);

                if (currentArrow != null)
                {
                    ShootArrow();
                    SoundManager.SelectSound(SoundType.BOW, 1, 0.2f);
                }
            }
        }
    }

    public void PauseBow()
    {
        bow.speed = 0f;
    }
    public void CreateArrow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null) return;

        Quaternion spawnRotation = arrowSpawnPoint.rotation * Quaternion.Euler(0f, -90f, -90f);

        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, spawnRotation);
        currentArrow.transform.SetParent(arrowSpawnPoint, true);
        Debug.Log("CURRENT ARROW WITH DAMAGE:" + currentArrow.GetComponent<Arrow>().damage);
        currentArrowRb = currentArrow.GetComponent<Rigidbody>();
        if (currentArrowRb == null)
            currentArrowRb = currentArrow.AddComponent<Rigidbody>();

        currentArrowRb.isKinematic = true;
        Physics.IgnoreCollision(
            currentArrow.GetComponent<Collider>(),
            player.GetComponent<Collider>()
        );
    }

    public void BowPul()
    {
        SoundManager.SelectSound(SoundType.BOW, 0, 0.5f);
    }

    public void ShootArrow()
    {
        if (currentArrow == null) return;
        currentArrow.transform.SetParent(null, true);
        currentArrowRb.isKinematic = false;
        currentArrowRb.velocity = arrowSpawnPoint.forward * arrowSpeed + Vector3.up * 1f;
        currentArrow = null;
        currentArrowRb = null;
    }

    public void LowHit()
    {
        currentArrow.GetComponent<Arrow>().LowHit();
    }

    public void MediumHit()
    {
        currentArrow.GetComponent<Arrow>().MediumHit();
    }

    public void HighHit()
    {
        currentArrow.GetComponent<Arrow>().HighHit();
    }
}