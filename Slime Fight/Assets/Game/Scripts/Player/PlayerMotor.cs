using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;

    // Movement Settings
    public float speed = 5f;
    public float defaultSpeed = 5f;
    public float gravity = -9.8f;

    // Jump Settings
    public float defaultJumpHeight = 1f;
    public float jumpHeight = 1f;

    // Crouch Settings
    private bool lerpCrouch;
    private float crouchTimer;
    private bool crouching;
    private bool sprinting;

    // Movement Lock
    public bool canMove = true;

    [Header("Alembic impact playback")]
    [SerializeField] private AlembicAutoPlay alembicAutoPlay;
    [Tooltip("Trigger when the distance from the bottom of the CharacterController to the ground is <= this value (meters)")]
    [SerializeField] private float groundTriggerDistance = 0.1f;

    // Prevent multiple triggers during the same fall
    private bool alembicPlayedThisFall;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            canMove = !canMove;

            if (canMove)
            {
                // Lock cursor
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                // Unlock cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (controller == null)
            return;

        // Check ground
        isGrounded = controller.isGrounded;

        // Reset trigger when player is grounded again
        if (isGrounded)
        {
            alembicPlayedThisFall = false;
        }
        else
        {
            // Only trigger while falling and only when the bottom of the controller is within threshold
            if (!alembicPlayedThisFall && alembicAutoPlay != null && playerVelocity.y < 0f)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                {
                    // Distance from the bottom of the controller to the ground
                    float distanceFromBottomToGround = hit.distance - controller.bounds.extents.y;

                    // Play only when the distance is <= the threshold
                    if (distanceFromBottomToGround <= groundTriggerDistance)
                    {
                        alembicAutoPlay.PlayOnce();
                        alembicPlayedThisFall = true;
                        Debug.Log($"Alembic triggered. distanceFromBottomToGround={distanceFromBottomToGround:F3}");
                    }
                }
            }
        }

        // Handle crouch lerp
        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1f;
            p *= p;

            if (crouching)
                controller.height = Mathf.Lerp(controller.height, 1f, p);
            else
                controller.height = Mathf.Lerp(controller.height, 2f, p);

            if (p > 1f)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
    }

    public void ProcessMove(Vector2 input)
    {
        if (!canMove) return;

        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (!canMove) return;
        if (isGrounded)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    public void Crouch()
    {
        if (!canMove) return;

        crouching = !crouching;
        crouchTimer = 0f;
        lerpCrouch = true;

        // Reset jump height first
        jumpHeight = defaultJumpHeight;

        if (crouching && sprinting)
        {
            speed = 2f;
            jumpHeight /= 2f;
        }
        else if (crouching)
        {
            speed = 1f;
            jumpHeight /= 2f;
        }
        else
        {
            speed = defaultSpeed;
        }
    }

    public void Sprint()
    {
        if (!canMove) return;

        sprinting = !sprinting;

        // Reset jump height first
        jumpHeight = defaultJumpHeight;

        if (sprinting && crouching)
        {
            speed = 2f;
            jumpHeight /= 2f;
        }
        else if (sprinting)
        {
            speed = 15f;
        }
        else
        {
            speed = defaultSpeed;
        }
    }
}