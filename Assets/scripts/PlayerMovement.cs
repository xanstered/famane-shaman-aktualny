using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    [HideInInspector] public bool isCrouching;

    [Header("Sprinting")]
    public float sprintSpeed;
    private bool isSprinting;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip crouchSound;

    private float footstepTimer = 0f;
    private float footstepInterval = 0.4f; 

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.8f, whatIsGround);
    
        MyInput();
        SpeedControl();
        HandleFootsteps();

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void HandleFootsteps()
    {
        if (grounded && (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f))
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0)
            {
                // interwa³ krokow zalezy tu od predkosci
                if (isSprinting)
                    footstepInterval = 0.5f;
                else if (isCrouching)
                    footstepInterval = 1.3f;
                else
                    footstepInterval = 0.6f;

                PlayFootstepSound();

                // reset timera
                footstepTimer = footstepInterval;
            }
        }
    }

    private void PlayFootstepSound()
    {
        if (walkSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(walkSound, isCrouching ? 0.5f : 1.0f);
        }
    }

    private void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    private void PlayCrouchSound()
    {
        if (crouchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(crouchSound);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            PlayJumpSound();
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey) && grounded)
        {
            PlayCrouchSound();
            StartCrouch();
        }

        if (Input.GetKeyUp(crouchKey) && grounded)
        {
            PlayCrouchSound();
            StopCrouch();
        }

        if (Input.GetKeyDown(sprintKey) && !isCrouching)
        {
            isSprinting = true;
        }

        if (Input.GetKeyUp(sprintKey) || isCrouching)
        {
            isSprinting = false;
        }
    }

    private void StartCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        isCrouching = true;
        isSprinting = false;
    }

    private void StopCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        isCrouching = false;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        float currentMoveSpeed;
        if (isCrouching)
            currentMoveSpeed = crouchSpeed;
        else if (isSprinting)
            currentMoveSpeed = sprintSpeed;
        else
            currentMoveSpeed = moveSpeed;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            float airSpeed = isCrouching ? crouchSpeed : moveSpeed;
            rb.AddForce(moveDirection.normalized * airSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        float maxSpeed;
        if (isCrouching)
            maxSpeed = crouchSpeed;
        else if (isSprinting)
            maxSpeed = sprintSpeed;
        else
            maxSpeed = moveSpeed;

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
       if (isCrouching)
        {
            StopCrouch();
        }
        
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
