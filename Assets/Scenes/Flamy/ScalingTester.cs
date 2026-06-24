using UnityEngine;
using UnityEngine.InputSystem;

public class ScalingTester : MonoBehaviour
{
    [SerializeField] private int xpAmountToGive = 500;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.AddXp(xpAmountToGive);
                Debug.Log("Added XP! Current Player Level: " + LevelManager.Instance.CurrentLevel);
            }
        }
    }
}