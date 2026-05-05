using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public void LoadingScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }
}
