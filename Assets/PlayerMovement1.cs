using UnityEngine;

public class PlayerMovement1 : MonoBehaviour
{
    public float speed = 5f;
    public Transform cameraTransform;

    void Update()
    {
        // Pobieranie danych z klawiatury (WSAD / Strza³ki)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Obliczanie kierunku ruchu wzglêdem kamery
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0; // Blokujemy ruch w górê/dó³
        right.y = 0;

        Vector3 moveDirection = (forward * moveZ + right * moveX).normalized;

        // Poruszanie obiektem
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }
}