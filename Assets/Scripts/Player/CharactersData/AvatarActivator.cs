using UnityEngine;

public class AvatarActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] avatarObjects;

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
    }
}