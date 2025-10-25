using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class PauseMenu : MonoBehaviour
{
    [SerializeField, Tooltip("The pause menu panel.")]
    private GameObject pauseMenuUI;

    public static bool isPaused = false;
    void Awake()
    {
        if (Time.timeScale == 0f) { Time.timeScale = 1f; }
    }
    // Pauses / Unpauses the game based on whether already <isPaused>, when esc is pressed.
    void Update()
    {
        bool escPressed = Input.GetKeyDown(KeyCode.Escape);
        if (!escPressed) { return; }

        if (isPaused) { Resume(); }
        else { Pause(); }
    }

    // Hides <pauseMenuUI>, unfreezes time, and updates <isPaused>.
    private void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Time restored!");
    }

    // Unhides <pauseMenuUI>, freezes time, and updates <isPaused>.
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Time paused!");
    }
}
