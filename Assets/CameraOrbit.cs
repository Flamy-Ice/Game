using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // Twoja kostka
    public float distance = 5.0f; // Odleg³oœæ od kostki
    public float sensitivity = 3.0f; // Czu³oœæ myszy

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // Ukrycie kursora myszy
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Pobieranie ruchu myszy
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;

        // Ograniczenie rotacji góra/dó³, ¿eby nie "fikaæ kozio³ków"
        rotationY = Mathf.Clamp(rotationY, -30f, 60f);

        // Obliczanie nowej rotacji i pozycji
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        transform.rotation = rotation;
        transform.position = position;
    }
}