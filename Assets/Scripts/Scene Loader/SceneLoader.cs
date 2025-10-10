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
        if (Instance != null && Instance != this)
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
        yield return StartCoroutine(Fade(1f));

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(Fade(0f));
    }

    // Fades <fadeCanvasGroup> from current alpha value to <targetAlpha>
    private IEnumerator Fade(float targetAlpha)
    {
        fadeCanvasGroup.blocksRaycasts = true;

        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float updatedAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeCanvasGroup.alpha = updatedAlpha;
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        fadeCanvasGroup.blocksRaycasts = targetAlpha != 0f;
    }
}
