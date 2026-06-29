using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NekoJump : MonoBehaviour
{
    [SerializeField] private Transform[] visualTransforms;
    [SerializeField] private float jumpVisualHeight = 0.4f;
    [SerializeField] private float animationSpeed = 14f;

    private CharacterController controller;
    private Vector3[] initialLocalPositions;
    private float waveCycle;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if ((visualTransforms == null || visualTransforms.Length == 0) && transform.childCount > 0)
        {
            visualTransforms = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                visualTransforms[i] = transform.GetChild(i);
            }
        }

        if (visualTransforms != null)
        {
            initialLocalPositions = new Vector3[visualTransforms.Length];
            for (int i = 0; i < visualTransforms.Length; i++)
            {
                if (visualTransforms[i] != null)
                {
                    initialLocalPositions[i] = visualTransforms[i].localPosition;
                }
            }
        }
    }

    void Update()
    {
        if (controller == null || visualTransforms == null || visualTransforms.Length == 0) return;

        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        bool isMoving = horizontalVelocity.sqrMagnitude > 0.01f;

        if (controller.isGrounded && isMoving)
        {
            waveCycle += Time.deltaTime * animationSpeed;
            if (waveCycle > Mathf.PI * 2f)
            {
                waveCycle -= Mathf.PI * 2f;
            }

            float localYOffset = Mathf.Abs(Mathf.Sin(waveCycle)) * jumpVisualHeight;

            for (int i = 0; i < visualTransforms.Length; i++)
            {
                if (visualTransforms[i] != null)
                {
                    visualTransforms[i].localPosition = new Vector3(
                        initialLocalPositions[i].x,
                        initialLocalPositions[i].y + localYOffset,
                        initialLocalPositions[i].z
                    );
                }
            }
        }
        else
        {
            waveCycle = 0f;
            for (int i = 0; i < visualTransforms.Length; i++)
            {
                if (visualTransforms[i] != null)
                {
                    visualTransforms[i].localPosition = Vector3.MoveTowards(
                        visualTransforms[i].localPosition,
                        initialLocalPositions[i],
                        animationSpeed * Time.deltaTime
                    );
                }
            }
        }
    }
}