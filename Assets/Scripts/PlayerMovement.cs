using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public Camera playerCamera;

    public float speed = 12f;
    public float gravity = -16;
    public float jumpHeight = 4.5f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    bool isMoving;

    public int maxJumps = 2;      // total jumps allowed (1 = normal, 2 = double)
    private int jumpsLeft;

    public float walkSpeed = 12f;
    public float sprintSpeed = 20f;
    private bool isSprinting = false;

    private Vector3 lastPosition = new Vector3(0f,0f,0f);

    float bobSpeed = 10f, bobAmount = 0.05f;
    float defaultY;

    private Coroutine fovCoroutine;
    public float normalFOV = 60f;
    public float sprintFOV = 70f;
    public float fovTransitionDuration = 0.25f;
    private bool wasSprinting = false;


    public float coyoteTime = 0.2f;
    private float coyoteCounter;
    private int jumpsUsed = 0;
    private bool wasGroundedLastFrame = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        jumpsLeft = maxJumps;
        defaultY = playerCamera.transform.localPosition.y;

    }

    // Update is called once per frame
    void Update()
    {
        // --- 1) Ground check & only reset when falling ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // If we just landed, reset jump counter:
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            jumpsUsed = 0;
        }
        // If we just stepped off an edge, consume the first jump:
        else if (!isGrounded && wasGroundedLastFrame)
        {
            jumpsUsed = 1;
        }
        wasGroundedLastFrame = isGrounded;
        // --- 2) Handle walking/running input ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool sprinting = Input.GetKey(KeyCode.LeftShift) && (x!=0 || z!=0);
        if (sprinting != wasSprinting)
        {
            if (fovCoroutine != null) StopCoroutine(fovCoroutine);
            float targetFOV = sprinting ? sprintFOV : normalFOV;
            fovCoroutine = StartCoroutine(AdjustFOV(playerCamera.fieldOfView, targetFOV, fovTransitionDuration));
            wasSprinting = sprinting;
        }
        float targetSpeed = sprinting ? sprintSpeed : walkSpeed;

        // --- 3) Movement on XZ plane ---
        Vector3 camF = playerCamera.transform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = playerCamera.transform.right;   camR.y = 0; camR.Normalize();
        Vector3 move = camR * x + camF * z;
        controller.Move(move * targetSpeed * Time.deltaTime);

        // --- 4) Jumping (double-jump) ---
        if (Input.GetButtonDown("Jump") && jumpsUsed < 2)
        {
            float strength = (jumpsUsed == 0) ? jumpHeight : jumpHeight * 0.75f;
            velocity.y = Mathf.Sqrt(strength * -2f * gravity);
            jumpsUsed++;
        }

        // --- 5) Gravity & final move ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        // Head-bob based on isMoving...
         if (isMoving && isGrounded) {
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultY + Mathf.Sin(Time.time * bobSpeed) * bobAmount,
                playerCamera.transform.localPosition.z
            );
        } else {
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                new Vector3(playerCamera.transform.localPosition.x, defaultY, playerCamera.transform.localPosition.z),
                Time.deltaTime * bobSpeed
            );
        }
    
    }

    private IEnumerator AdjustFOV(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            playerCamera.fieldOfView = Mathf.Lerp(from, to, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        playerCamera.fieldOfView = to;
    }

}
