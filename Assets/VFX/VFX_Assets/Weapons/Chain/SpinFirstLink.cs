using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpinFirstLink : MonoBehaviour
{
    [Header("Spin Settings")]
    [Tooltip("How fast the chain spins (degrees per second). Faster speeds make the head lift higher.")]
    public float spinSpeed = 400f;

    [Tooltip("The axis to spin around. Try (0, 1, 0) to swing like a lasso, or (0, 0, 1) to swing like a windmill.")]
    public Vector3 spinAxis = new Vector3(0, 1, 0);

    private Rigidbody rb;

    void Start()
    {
        // Grab the Rigidbody attached to this first link
        rb = GetComponent<Rigidbody>();

        // Double-check that this anchor is Kinematic (as done at the end of your tutorial)
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void FixedUpdate()
    {
        // Smoothly rotate the physical body. This is the correct way to rotate 
        // joint anchors so that the attached chain calculates velocity correctly.
        if (rb != null)
        {
            Quaternion deltaRotation = Quaternion.Euler(spinAxis * spinSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }
}