using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [Header("Parametry Headbob")]
    [SerializeField] private float bobFrequency = 5f;
    [SerializeField] private float bobHorizontalAmplitude = 0.05f;
    [SerializeField] private float bobVerticalAmplitude = 0.08f;
    [SerializeField][Range(0, 1)] private float headBobSmoothing = 0.1f;  // wyg³adzanie
    [SerializeField] private bool enableHeadBob = true;


    private PlayerMovement playerMovement;
    private Rigidbody playerRigidbody;

    private float walkingTime = 0;
    private Vector3 targetCameraPosition;
    private Vector3 originalLocalPosition;
    private float currentSpeed;

    private void Start()
    {
        originalLocalPosition = transform.localPosition;
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerRigidbody = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        if (!enableHeadBob) return;

        CheckMotion();
        ResetPosition();
        if (currentSpeed > 0.1f)
            ApplyHeadBob();
    }

    private void CheckMotion()
    {
        Vector3 playerVelocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
        currentSpeed = playerVelocity.magnitude;
    }

    private void ApplyHeadBob()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");


        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            // efekt zwiekszany przez sprintowanie
            float speedMultiplier = 1f;
            if (Input.GetKey(playerMovement.sprintKey) && !playerMovement.isCrouching)
            {
                speedMultiplier = 1.5f;
            }

            //efekt sie zmniejsza przy kucaniu
            else if (playerMovement.isCrouching)
            {
                speedMultiplier = 0.5f;
            }

            // aktualizacja czasu chodzenia
            walkingTime += Time.deltaTime * bobFrequency * speedMultiplier;


            float horizontalBob = Mathf.Sin(walkingTime) * bobHorizontalAmplitude * speedMultiplier;
            float verticalBob = Mathf.Sin(walkingTime * 2) * bobVerticalAmplitude * speedMultiplier;

            targetCameraPosition = originalLocalPosition + new Vector3(horizontalBob, verticalBob, 0);
        }
        else
        {
            walkingTime = 0;
            targetCameraPosition = originalLocalPosition;
        }


        transform.localPosition = Vector3.Lerp(transform.localPosition, targetCameraPosition, headBobSmoothing);
    }

    private void ResetPosition()
    {
        if (transform.localPosition == originalLocalPosition) return;

        if (currentSpeed < 0.1f)
        {
            targetCameraPosition = originalLocalPosition;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPosition, headBobSmoothing * 2);
        }
    }
}