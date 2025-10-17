using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class PauseMenu : MonoBehaviour
{
    [SerializeField, Tooltip("The pause menu panel.")]
    private GameObject pauseMenuUI;

    private static bool isPaused = false;

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
    }

    // Unhides <pauseMenuUI>, freezes time, and updates <isPaused>.
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
}
