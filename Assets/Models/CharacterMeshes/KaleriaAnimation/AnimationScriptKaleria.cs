using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationScryptKaleria : MonoBehaviour
{
    private Animator KaleriasAnimator;
    private bool isMoving = false;

    void Start()
    {
        KaleriasAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        HandleMovementAnimation();
        HandleJumpAnimation();
    }

    void HandleMovementAnimation()
    {
        // Check if ANY of the WASD keys are currently being held down
        bool isPressingWASD = Keyboard.current.wKey.isPressed ||
                             Keyboard.current.aKey.isPressed ||
                             Keyboard.current.sKey.isPressed ||
                             Keyboard.current.dKey.isPressed;

        // 1. If they just started pressing WASD and weren't moving before -> Trigger RUN
        if (isPressingWASD && !isMoving)
        {
            isMoving = true;
            KaleriasAnimator.SetTrigger("KaleriaRun");
        }
        // 2. If they let go of ALL WASD keys and were moving -> Trigger IDLE (via KaleriaRun trigger)
        else if (!isPressingWASD && isMoving)
        {
            isMoving = false;
            KaleriasAnimator.SetTrigger("KaleriaRun");
        }
    }

    void HandleJumpAnimation()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            KaleriasAnimator.SetTrigger("KaleriaJump");
        }
    }
}