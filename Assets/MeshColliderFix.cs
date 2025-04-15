using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderFix : MonoBehaviour
{
    private Rigidbody rb;
    private MeshCollider meshCollider;

    public bool forceConvex = true;
    public bool useGravity = true;
    public float mass = 10f;
    public float drag = 0.5f;
    public float angularDrag = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshCollider = GetComponent<MeshCollider>();

        if (meshCollider != null)
        {
            meshCollider.convex = forceConvex;

            // if mesh is too complex, mo¿na spróbowaæ uproœciæ kolizje
            meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning |
                                          MeshColliderCookingOptions.CookForFasterSimulation;
        }

        if (rb != null)
        {
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.useGravity = useGravity;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }
    void FixedUpdate()
    {
        if (rb != null && rb.velocity.magnitude > 0.1f)
        {
            Vector3 horizontalVelocity = rb.velocity;
            horizontalVelocity.y = 0;

            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 2f);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (rb != null)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                rb.AddForceAtPosition(contact.normal * 0.5f, contact.point, ForceMode.Impulse);
            }
        }
    }
}
