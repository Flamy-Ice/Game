using UnityEngine;

public class OrbitScript : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The object this object will orbit around.")]
    public Transform target;

    [Header("Orbit Settings")]
    [Tooltip("Speed of the orbit in degrees per second.")]
    public float speed = 50.0f;

    [Tooltip("The axis around which the object will orbit.")]
    public Vector3 orbitAxis = Vector3.up;

    void Update()
    {
        // Ensure we actually have a target to orbit
        if (target != null)
        {
            // Rotate this object around the target's position, along the specified axis, at the given speed
            transform.RotateAround(target.position, orbitAxis, speed * Time.deltaTime);
        }
    }
}