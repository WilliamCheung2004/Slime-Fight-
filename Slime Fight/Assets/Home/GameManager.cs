using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scene to load")]
    [SerializeField] private string targetScene;

    public void ExitGame()
    {
        Debug.Log("Exiting the game...");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(targetScene);
    }

}