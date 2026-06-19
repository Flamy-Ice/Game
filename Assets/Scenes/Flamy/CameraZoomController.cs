using UnityEngine;
using Unity.Cinemachine;

public class CameraZoomController : MonoBehaviour
{
    public CinemachineCamera mainCamera;
    public CinemachineCamera zoomCamera;

    private bool isZoomed = false;

    // Tę funkcję podepniesz pod przycisk UI
    public void ToggleZoom()
    {
        isZoomed = !isZoomed;

        if (isZoomed)
        {
            mainCamera.Priority = 10;
            zoomCamera.Priority = 20;
        }
        else
        {
            mainCamera.Priority = 20;
            zoomCamera.Priority = 10;
        }
    }
}