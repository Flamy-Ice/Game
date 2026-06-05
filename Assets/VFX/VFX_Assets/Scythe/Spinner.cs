using UnityEngine;

public class Spinner : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Degrees per second around the X, Y, and Z axes")]
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f); // Default: spins on the Y-axis

    void Update()
    {
        // Rotate the object every frame, independent of frame rate
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}