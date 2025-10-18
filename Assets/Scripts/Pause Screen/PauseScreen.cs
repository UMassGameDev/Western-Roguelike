using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class PauseMenu : MonoBehaviour
{
    [SerializeField, Tooltip("The pause menu panel.")]
    private GameObject pauseMenuUI;

    public static bool isPaused = false;
    
    // Instance of PauseMenu, available to all other scripts:
    public static PauseMenu Instance { get; private set; }

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

        // Ensure the menu starts hidden.
        if (pauseMenuUI != null) { pauseMenuUI.SetActive(false); }
    }


    private void OnEnable()
    {
        // Disable if this is the main menu scene.
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "MainMenu")
        {
            gameObject.SetActive(false);
            isPaused = false;
        }
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
    }

    // Unhides <pauseMenuUI>, freezes time, and updates <isPaused>.
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
}
