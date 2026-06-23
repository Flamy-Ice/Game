using System.Collections;
using UnityEngine;

public class HammerSlam : MonoBehaviour
{
    public enum RotationAxis { X_Axis, Y_Axis, Z_Axis }

    [Header("Hierarchy References")]
    [Tooltip("Drag the 'FlamingoMalletMesh' object here")]
    [SerializeField] private GameObject malletMesh;

    [Tooltip("Drag the hidden 'POW' child object here")]
    [SerializeField] private GameObject powVFX;

    [Header("Axis Settings")]
    [SerializeField] private RotationAxis slamAxis = RotationAxis.X_Axis;
    [SerializeField] private bool invertRotation = false;

    [Header("Rotation Settings")]
    [SerializeField] private float slamAngle = 90f;
    [SerializeField] private float slamSpeed = 5f;
    [SerializeField] private float vfxDuration = 1.5f; // How long the POW stays before total destruction

    private Quaternion originalRotation;
    private Quaternion targetRotation;

    void Start()
    {
        originalRotation = transform.localRotation;

        float finalAngle = invertRotation ? -slamAngle : slamAngle;

        Vector3 rotationVector = Vector3.zero;
        switch (slamAxis)
        {
            case RotationAxis.X_Axis:
                rotationVector = new Vector3(finalAngle, 0, 0);
                break;
            case RotationAxis.Y_Axis:
                rotationVector = new Vector3(0, finalAngle, 0);
                break;
            case RotationAxis.Z_Axis:
                rotationVector = new Vector3(0, 0, finalAngle);
                break;
        }

        targetRotation = originalRotation * Quaternion.Euler(rotationVector);

        // Make sure POW starts hidden just in case
        if (powVFX != null) powVFX.SetActive(false);

        StartCoroutine(AutomaticSlamSequence());
    }

    IEnumerator AutomaticSlamSequence()
    {
        float elapsed = 0f;

        // 1. Slam Down
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * slamSpeed;
            transform.localRotation = Quaternion.Slerp(originalRotation, targetRotation, elapsed);
            yield return null;
        }

        transform.localRotation = targetRotation;

        // 2. Hide Mallet & Show POW
        if (malletMesh != null)
        {
            malletMesh.SetActive(false); // This hides the mesh, armature, cylinder, etc.
        }

        if (powVFX != null)
        {
            powVFX.transform.SetParent(null); // Detach POW so it doesn't vanish when we hide the mesh
            powVFX.SetActive(true);           // Show the POW!
        }

        // 3. Clean up everything from memory after it finishes playing
        yield return new WaitForSeconds(vfxDuration);

        if (powVFX != null) Destroy(powVFX);
        Destroy(gameObject); // Destroys the leftover empty prefab parent
    }
}