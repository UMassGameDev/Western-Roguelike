using UnityEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneLoader.Instance.LoadSceneWithFade("MainScene");
    }

    public void Quit()
    {
        // This is so quitting is the same in the editor as it is in the build:
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
