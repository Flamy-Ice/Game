using UnityEngine;

public class CrownBillboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Awake()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            transform.forward = mainCameraTransform.forward;
        }
    }
}