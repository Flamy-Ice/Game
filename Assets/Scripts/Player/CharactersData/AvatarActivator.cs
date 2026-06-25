using UnityEngine;

public class AvatarActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] avatarObjects;
    [SerializeField] private GameObject[] gameOverAvatarObjects;

    void Start()
    {
        int index = CharacterTransfer.SelectedIndex;

        for (int i = 0; i < avatarObjects.Length; i++)
        {
            if (avatarObjects[i] != null)
            {
                avatarObjects[i].SetActive(i == index);
            }
        }

        for (int i = 0; i < gameOverAvatarObjects.Length; i++)
        {
            if (gameOverAvatarObjects[i] != null)
            {
                gameOverAvatarObjects[i].SetActive(i == index);
            }
        }
    }
}