using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 7f; // Si³a skoku
    private Rigidbody rb;
    private bool isGrounded; // Czy kostka dotyka ziemi?

    void Start()
    {
        // Pobieramy komponent Rigidbody z kostki
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Jeœli naciœniesz Spacjê ORAZ kostka stoi na ziemi
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Dodajemy si³ê skierowan¹ w górê (Impulse to nag³y skok)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Po skoku ju¿ nie jesteœmy na ziemi
        }
    }

    // Wykrywanie kolizji z pod³o¿em
    void OnCollisionStay(Collision collision)
    {
        // Jeœli dotykamy czegokolwiek (np. pod³ogi), mo¿emy skakaæ
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        // Kiedy przestajemy dotykaæ pod³ogi
        isGrounded = false;
    }
}