using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public Transform Camera;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f,0f,0f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // groung check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // resset velocity
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //creating the movement vector
        Vector3 move = Camera.transform.right * x + Camera.transform.forward * z;
        //moving player
        controller.Move(move * speed * Time.deltaTime);

        //check if player can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // jump 
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //fall 
        velocity.y += gravity * Time.deltaTime;

        // execut the jump
        controller.Move(velocity * Time.deltaTime);
    
        if (lastPosition != gameObject.transform.position && isGrounded == true)
        {
            isMoving = true;
            //mn be3d
        }
        else
        {
            isMoving = false;
            //men be3d
        }
    
        lastPosition = gameObject.transform.position;
    
    }

}
