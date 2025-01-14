using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] Transform orientation;

    [Header("Wall Running")]
    [SerializeField] float distanceToWall = 0.5f;
    [SerializeField] float minimumJumpHeight = 1.5f;
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;

    public float tilt;
    bool wallLeft = false;
    bool wallRight = false;
    bool onWall = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        CheckWall();

        if(CanWallRun())
        {
            if (wallLeft)
            {
                StartWallRun();
            }
            else if (wallRight)
            {
                StartWallRun();
            }
            else
            {
                StopWallRun();
            }
        }else if(!CanWallRun() && Mathf.Abs(tilt) > 0.01f)
        {
            tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
            rb.useGravity = true;
        }
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit,distanceToWall);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit,distanceToWall);
        
        
    }

    public bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, 2);
    }

    void StartWallRun() 
    {

        rb.useGravity = false;


        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        

        if (wallLeft)
        {
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        } 
        else if(wallRight)
        {
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(wallLeft)
            {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            else if(wallRight)
            {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
        }

    }

    void StopWallRun()
    {

        rb.useGravity = true;
        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
        
    }
}
