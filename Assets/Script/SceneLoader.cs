using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Assign scene name in Inspector
    public string sceneName;

    // Call this from UI Button
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}