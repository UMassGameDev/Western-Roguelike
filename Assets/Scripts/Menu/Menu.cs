using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     Class that stores all menu functionality.
/// </summary>
public class Menu : MonoBehaviour
{
    /// <summary>
    ///     Starts the game w/fade effect.
    /// </summary>
    /// <remarks>
    ///     Currently, this just loads MainScene.unity.
    ///     Update when save files are implemented.
    /// </remarks>
    /// <example>
    ///     Menu.PlayGame();
    /// </example>
    /// <seealso cref="SceneLoader.LoadSceneWithFade(string)"/>
    /// <seealso cref="Menu.Quit()"/>
    public static void PlayGame()
    {
        SceneLoader.Instance.LoadSceneWithFade("MainScene");
    }

    /// <summary>
    ///     Quits the application.
    /// </summary>
    /// <remarks>
    ///     Quits to desktop, not main menu. 
    ///     Works both in the editor as well as in the build.
    /// </remarks>
    /// <seealso cref="Menu.PlayGame()"/> 
    public static void Quit()
    {
        // This is so quitting is the same in the editor as it is in the build:
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
