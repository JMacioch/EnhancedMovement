using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float groundDrag = 2f;
    public float playerHeight = 2f;
    public float maxSlopeAngle = 10f;
    public Transform orientation;
    float moveSpeed;

    [Header("Sprinting")]
    public float runSpeed;
    public float acceleration;   

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float airMovementMultiplier = 4f;
    public float airDrag = 1f;
    float movementMultiplier = 10f;
    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    private bool isGrounded;
    private float distanceToGround = 0.3f;

    [Header("HeadBobbing")]
    [SerializeField] float walkBobSpeed;
    [SerializeField] float walkBobAmount;

    [Header("Ladder")]
    [SerializeField] LayerMask ladderMask;
    bool isLadder = false;

    [Header("Crouch and Slide")]
    [SerializeField] CapsuleCollider collider;
    Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    Vector3 playerScale;
    [SerializeField] float slideForce = 100;
    
    

    Rigidbody rb;
    Vector3 moveDirection;
    public Camera cam;
    float angle;
    public WallRun wallruning;
    bool Jumped = false;
    bool wasInAir = false;
    [SerializeField] ParticleSystem landingParticle;

    Vector3 slopeMoveDirection;
    RaycastHit slopeHit;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Application.targetFrameRate = 200;
        moveSpeed = walkSpeed;
        playerScale = transform.localScale;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, distanceToGround, groundMask);

        if (!isGrounded && rb.velocity.y < -8)
            wasInAir = true;

        MyInput();
        DragControl();
        Sprinting();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jumped = true;
            Jump();
            
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            StopCrouch();
        }
       
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        if (isGrounded && wasInAir && rb.velocity.y < -8)
        {
            Landing();
        }


    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void Landing()
    {
        Debug.Log("WYLADAWAL");
        wasInAir = false;
        if(Input.GetKey(KeyCode.C))
        landingParticle.Play();
        else
        Instantiate(landingParticle, transform.position - new Vector3(0,0.8f,0), Quaternion.Euler(-90,0,0));
        
        
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void DragControl()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    void Sprinting()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, acceleration * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 80, 5 * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 70, 5 * Time.deltaTime);

        }
    }

    void Crouch()
    {
        collider.radius = 0.5f;
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 10f)
        {
            if (true)
            {
                rb.AddForce(orientation.transform.forward * slideForce);
                Debug.Log(rb.velocity.magnitude);
                movementMultiplier = 0;
                groundDrag = 1f;

            }
        }
        else
            movementMultiplier = 5f;

    }

    void StopCrouch()
    {
        collider.radius = 0.6f;
        transform.localScale = playerScale;
        movementMultiplier = 10;
        groundDrag = 6f;
    }


    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
      

    }


    void MovePlayer()
    {

        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }else if(isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMovementMultiplier, ForceMode.Acceleration);
        }

        if(!isGrounded && !wallruning.CanWallRun())
        {
            rb.useGravity = true;
            Jumped = false;
            
        }
       
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            

           angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if (angle < maxSlopeAngle && angle > 0.1f)
            {
                
                rb.useGravity = false;
                if(!Jumped)
                {
                    rb.AddForce(-slopeHit.normal * 50, ForceMode.Force);
                    Debug.Log("DODAJE SILKE");
                }
                
                

            }else rb.useGravity = true;



            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;

    }



}

// Add Physics material (to player and all objects collider) to avoid stucking in a walls
