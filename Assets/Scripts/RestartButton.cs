using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void RestartGame()
    {
        SaveManager.Instance.ResetSave();
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
