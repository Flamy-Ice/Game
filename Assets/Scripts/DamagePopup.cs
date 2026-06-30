using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float cameraOffset = 1.2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip enemyDamageSound;
    [SerializeField] private AudioClip enemyCritSound;
    [SerializeField] private AudioClip playerDamageSound;
    [SerializeField] private AudioClip playerDodgeSound;
    [SerializeField][Range(0f, 1f)] private float soundVolume = 0.1f;

    private TextMeshPro textMesh;
    private float disappearTimer = 0.6f;
    private Color textColor;
    private float moveYSpeed = 2f;
    private Transform mainCameraTransform;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    public void Setup(float damageAmount, bool isCrit, bool isPlayerDamage = false, bool isDodge = false)
    {
        if (mainCameraTransform != null)
        {
            transform.position -= mainCameraTransform.forward * cameraOffset;
        }

        if (isDodge)
        {
            textMesh.SetText("Dodge");
            textMesh.color = Color.blue;
            PlaySound(playerDodgeSound);
        }
        else if (isPlayerDamage)
        {
            int roundedDamage = Mathf.RoundToInt(damageAmount);
            textMesh.SetText("-" + roundedDamage.ToString());
            textMesh.color = Color.red;
            PlaySound(playerDamageSound);
        }
        else
        {
            int roundedDamage = Mathf.RoundToInt(damageAmount);
            textMesh.SetText(roundedDamage.ToString());
            if (isCrit)
            {
                textMesh.color = Color.yellow;
                PlaySound(enemyCritSound);
            }
            else
            {
                PlaySound(enemyDamageSound);
            }
        }

        textColor = textMesh.color;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, soundVolume);
        }
    }

    private void Update()
    {
        transform.position += new Vector3(0, moveYSpeed, 0) * Time.deltaTime;
        disappearTimer -= Time.deltaTime;

        if (mainCameraTransform != null)
        {
            transform.forward = mainCameraTransform.forward;
        }

        if (disappearTimer < 0)
        {
            float fadeSpeed = 5f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}