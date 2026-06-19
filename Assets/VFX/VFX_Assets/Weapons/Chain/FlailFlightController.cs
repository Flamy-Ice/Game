using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlailFlightController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your spinning FIRST LINK (the anchor) here.")]
    public Transform centerAnchor;

    [Header("Constant Forces (Always Active)")]
    [Tooltip("How hard to constantly push the head outward away from the center.")]
    public float outwardForce = 15f;

    [Tooltip("A gentle upward force to counteract gravity and keep the head floating high.")]
    public float upwardForce = 8f;

    [Header("Sideways Speed Control")]
    [Tooltip("How hard to constantly push the head sideways to maintain orbital speed.")]
    public float sideForce = 10f;

    [Tooltip("The maximum orbital speed. Sideways force shuts off if it goes faster than this.")]
    public float maxSideSpeed = 20f;

    [Tooltip("Invert this if the sideways push is fighting against the anchor's rotation direction.")]
    public bool reverseSideDirection = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (centerAnchor == null)
        {
            Debug.LogWarning("FlailFlightController: Please assign your spinning first link!");
        }
    }

    void FixedUpdate()
    {
        if (rb == null || centerAnchor == null) return;

        // 1. Calculate direction vectors
        Vector3 directionFromCenter = transform.position - centerAnchor.position;
        directionFromCenter.y = 0; // Keep horizontal math strictly flat
        Vector3 outwardDir = directionFromCenter.normalized;

        // 2. ALWAYS APPLY: Outward Push and Upward Lift (Never capped)
        Vector3 horizontalPush = outwardDir * outwardForce;
        Vector3 verticalLift = Vector3.up * upwardForce;
        rb.AddForce(horizontalPush + verticalLift, ForceMode.Force);

        // 3. CONDITIONALLY APPLY: Sideways Orbital Force
        // We isolate the sideways velocity vector using a dot product with the side direction
        Vector3 sideDir = Vector3.Cross(outwardDir, Vector3.up);
        if (reverseSideDirection)
        {
            sideDir = -sideDir;
        }

        // Measure how fast the head is moving strictly in the sideways direction
        float currentSideSpeed = Vector3.Dot(rb.linearVelocity, sideDir);

        // Only add sideways force if it hasn't hit the orbital speed limit
        if (currentSideSpeed < maxSideSpeed)
        {
            Vector3 sidewaysPush = sideDir * sideForce;
            rb.AddForce(sidewaysPush, ForceMode.Force);
        }
    }
}