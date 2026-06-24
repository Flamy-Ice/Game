using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float cameraOffset = 1.2f;

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

    public void Setup(float damageAmount, bool isCrit)
    {
        if (mainCameraTransform != null)
        {
            transform.position -= mainCameraTransform.forward * cameraOffset;
        }

        textMesh.SetText(Mathf.RoundToInt(damageAmount).ToString());

        if (isCrit)
        {
            textMesh.color = Color.yellow;
        }

        textColor = textMesh.color;
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