using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableBoxPushing : MonoBehaviour
{
    private Rigidbody rb;

    public float stabilityForce = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.freezeRotation = true;

            rb.centerOfMass = Vector3.zero;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            Vector3 targetRotation = new Vector3(0, currentRotation.y, 0);

            Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, Time.fixedDeltaTime * stabilityForce);

            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (rb != null)
        {
            Vector3 contactNormal = collision.contacts[0].normal;
            rb.AddForce(contactNormal * stabilityForce, ForceMode.Force);
        }
    }
}
