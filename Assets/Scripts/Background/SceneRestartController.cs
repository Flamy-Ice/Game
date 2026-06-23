using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestartController : MonoBehaviour
{
    public void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}