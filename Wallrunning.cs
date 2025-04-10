using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting;
using UnityEngine;

public class Wallrunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallRunForce;
    public float maxWallRunTime;
    private float WallRunTimer;
    public float wallClimbSpeed;

    [Header("Input")]
    public KeyCode JumpKey = KeyCode.Space;
    private float horizontalInput;
    private float verticalInput;
    private KeyCode upwardsRunKey = KeyCode.C;
    private KeyCode downwardsRunKey = KeyCode.V;
    private bool upwardsRunning;
    private bool downwardsRunning;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool leftWall;
    private bool rightWall;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();

        if (pm.wallRunning)
        {
            WallRunningMovement();
        }
    }

    private void FixedUpdate()
    {

    }

    private void CheckForWall()
    {
        rightWall = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        leftWall = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround() 
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine() 
    {
        // Getting inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // State 1 - Wallrunning
        if ((leftWall || rightWall) && verticalInput > 0 && AboveGround() && !exitingWall)
        { 
            if (!pm.wallRunning)
            {
                StartWallRun();
            }


            if (Input.GetKeyDown(JumpKey))
            {
                WallJump();
            }
        }

        // State 2 - Exiting
        else if (exitingWall) 
        {
            if (pm.wallRunning) 
            {
                StopWallRun();
            }

            if (exitWallTimer > 0) 
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <= 0) 
            {
                exitingWall = false;
            }
        }

        // State 3 - Nothing
        else 
        {
            if (pm.wallRunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun() 
    {
        pm.wallRunning = true;
    }

    private void WallRunningMovement() 
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.y);
        
        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude) 
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);

        if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        if (!(leftWall && horizontalInput > 0) && !(rightWall && horizontalInput < 0)) 
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    private void StopWallRun() 
    { 
        pm.wallRunning = false;
    }

    private void WallJump()
    {
        // Enter exit wall state
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
