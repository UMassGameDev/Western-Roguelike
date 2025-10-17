using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


/// <summary>
///     This is the global object that handles changing scenes
///     with effects.
/// </summary>
/// <remarks>
///     This object persists between scene changes and is globally
///     available.
/// </remarks>
public class SceneLoader : MonoBehaviour
{
    // These [] is an attribute, attributes mostly provide additional info in the editor.
    [Header("Fade Settings")]
    [SerializeField, Tooltip("Image in Canvas group to be faded in/out. Fades the CANVAS GROUP, not the IMAGE")]
    private CanvasGroup fadeCanvasGroup;
    [SerializeField, Tooltip("Duration of each fade in/out, x2 for both, in seconds. Default = 1s")]
    private float fadeDuration = 1f;

    // Instance of SceneLoader, available to all other scripts:
    public static SceneLoader Instance { get; private set; }

    // Checks that { 
    //    1:) there is only 1 <Instance>
    //    2:) it is this one
    // } then makes Instance persist across scenes:
    void Awake()
    {
        bool instanceExists = (Instance != null), instanceIsNotThis = (Instance != this); 

        if (instanceExists && instanceIsNotThis)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    ///     Loads scene denoted by <paramref name="sceneName"/> with
    ///     appropriate fade effects.
    /// </summary>
    /// <remarks>
    ///     This function does both the fade in and fade out.
    /// </remarks>
    /// <param name="sceneName">
    ///     Name of the scene to load, in format "SceneName" (no .unity)
    /// </param>
    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    // Fades out, loads scene denoted by <sceneName>, and fades back in.
    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return StartCoroutine(Fade(1f));  // Fade out.

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);  // Switch scene.
        while (!op.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(Fade(0f));  // Fade in.
    }

    // Fades <fadeCanvasGroup> from current alpha value to <targetAlpha> (alpha = transparency, 0 being invisible, 1 being visible).
    private IEnumerator Fade(float targetAlpha)
    {
        fadeCanvasGroup.blocksRaycasts = true;  // Disables most input during the duration of the fade.

        float initAlpha = fadeCanvasGroup.alpha, time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = CalculateCurrentAlpha(initAlpha, targetAlpha, time);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        fadeCanvasGroup.blocksRaycasts = targetAlpha != 0f;  // Re-enables the input if this fade was not a fade out.
    }

    //~(Helper Methods)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    // Interpolates between <initAlpha> & <targetAlpha>, based on the ratio of <currentTime> / <fadeDuration> (time passed out of duration).
    private float CalculateCurrentAlpha(float initAlpha, float targetAlpha, float currentTime) => Mathf.Lerp(initAlpha, targetAlpha, currentTime / fadeDuration);
}
