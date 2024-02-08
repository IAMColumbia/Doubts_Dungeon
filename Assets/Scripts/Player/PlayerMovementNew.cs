using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovementNew : MonoBehaviour
{
    // P R O P E R T I E S
    
    [Header("Movement Settings")]
    [SerializeField] float Speed = 8f;
    float SpeedMultiplier = 1000;
    [SerializeField] float rotationSpeed = 360.0f;
    [SerializeField] float dodgeDistance = 2.5f;
    [SerializeField] float dodgeDuration = 0.05f;
    [SerializeField] float decelerationFactor = 10.0f; // Adjust the value as needed

    [Header("Camera Properties")]
    [SerializeField]
    GameObject VirtualCamera;
    [SerializeField]
    float ShakeIntensity;
    [SerializeField]
    float totalShakeTime;
    float shakeTimer;

    

    [Header("Animation Settings")]
    [SerializeField] Animator playerAnimator;


    [Header("Misc")]
    Plane plane;

    [SerializeField]
    public InputType PlayerInputMode;

    [Header("References")]
    public ShootBehavior SB;
    private Rigidbody rb;
    CinemachineVirtualCamera CMVcam;
    [SerializeField]
    OnScreenConsole console;


    // U N I T Y  M E T H O D S
    // Start is called before the first frame update
    void Start()
    {
        plane = new Plane(Vector3.down, transform.position.y);
        CMVcam = VirtualCamera.GetComponent<CinemachineVirtualCamera>();
        playerAnimator = GetComponent<Animator>();
        SB = GetComponent<ShootBehavior>(); 
        rb = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
        CheckForDodge();
        HandleLookDirection();
    }


    // M I S C

    void HandleLookDirection()
    {
        switch(PlayerInputMode)
        {
            case InputType.Keyboard:
                LookAtMouse();
                break;

            case InputType.Controller:

                break;
        }
    }
    void LookAtMouse()
    {
        // turns the player to be looking at the mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 targetPosition = ray.GetPoint(distance);

            // Preserve the y-coordinate of the player
            // Calculate the direction only in the x and z plane, keeping y fixed
            Vector3 direction = (targetPosition - transform.position);
            direction.y = 0; // Keep y-coordinate fixed (flat plane)

            if (direction != Vector3.zero)
            {
                // Calculate the angle between the forward direction and the mouse direction
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                // Rotate the player around the Y-axis to face the mouse cursor
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));
            }
        }
    }
    void Move()
    {
        // moves the player 
        Vector3 move = GetMoveVector();
        playerAnimator.SetFloat("speed", move.magnitude);

        //Speed *= SpeedMultiplier;
        move *= Speed * Time.deltaTime;
        console.Log("Move Vector with Speed and Time Adjustment", move, 1);
        //rb.AddForce(move, ForceMode.Force);
        Vector3 movePosition = transform.position + move;
        transform.position = movePosition;
        console.Log("Player Position", transform.position, 2);
    }

    Vector3 GetMoveVector()
    {
        //calculates the move vector for the player 

        Vector3 move = Vector3.zero;

        switch (PlayerInputMode)
        {
            case InputType.Keyboard:
                move = (Input.GetAxis("Vertical") * getCameraForward()
                    + Input.GetAxis("Horizontal") * getCameraRight()).normalized;
                break;

            case InputType.Controller:
                move = (Input.GetAxis("LSVertical") * getCameraForward()
                    + Input.GetAxis("LSHorizontal") * getCameraRight()).normalized;
                break;
        }
        console.Log("Move Vector", move, 0);
        return move;
        
    }
    Vector3 getCameraForward()
    {
        // Calculate movement direction based on camera's perspective
        Vector3 cameraForward = Camera.main.transform.forward;
        // Ignore the y-component to stay in the x-z plane
        cameraForward.y = 0f;
        cameraForward.Normalize();
        return cameraForward;
    }
    Vector3 getCameraRight()
    {
        // Calculate movement direction based on camera's perspective
        Vector3 cameraRight = Camera.main.transform.right;

        // Ignore the y-component to stay in the x-z plane
        cameraRight.y = 0f;
        cameraRight.Normalize();
        return cameraRight;
    }

    void Dodge()
    {
        // Get input from WASD keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Construct dodge direction from input
        Vector3 dodgeDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        //raycast for dodge distatnce
        RaycastHit hit;
        Ray dodgeRay = new Ray(transform.position, dodgeDirection);
        //draws the ray for debug
        Debug.DrawRay(transform.position, dodgeDirection, Color.cyan, dodgeDuration);
        
        // if we hit something, the distance from us to the thing is the distance we dodge
        if (Physics.Raycast(dodgeRay, out hit, dodgeDistance))
        {
            float distance = hit.distance;

            // Calculate the dodge direction based on the player's current rotation
            // Move forward in the player's facing direction

            // Calculate the number of steps based on the duration
            int numSteps = Mathf.FloorToInt(dodgeDuration / Time.fixedDeltaTime);

            // Calculate the dodge step 
            //.2 is magic number, could get meshrender.bounds.extents.x,
            //      bassicly here to give a bit of buffer so we dont end up halfway inside things
            Vector3 dodgeStep = dodgeDirection * (distance - 0.2f) / numSteps;

            // Start the dodge coroutine
            StartCoroutine(PerformDodge(dodgeStep, numSteps));
        }
        // if we don't hit something, dodge the set dodge distance?
        else 
        {
            // Calculate the number of steps based on the duration
            int numSteps = Mathf.FloorToInt(dodgeDuration / Time.fixedDeltaTime);

            // Calculate the dodge step
            Vector3 dodgeStep = dodgeDirection * dodgeDistance / numSteps;

            // Start the dodge coroutine
            StartCoroutine(PerformDodge(dodgeStep, numSteps));
        }
    }

    void CheckForDodge()
    {
        if (Input.GetButtonDown("Dodge"))
        {
            Debug.Log("Player hit: Dodge | left shift key");
            //if (Stamina.UseStamina(25))
            //{
            //    return true;
            //}
            Dodge();
        }
        
    }

    private IEnumerator PerformDodge(Vector3 dodgeStep, int numSteps)
    {
        for (int i = 0; i < numSteps; i++)
        {

            transform.position += dodgeStep;
            // Wait for the next fixed update frame
            yield return new WaitForFixedUpdate();
        }
    }
}
