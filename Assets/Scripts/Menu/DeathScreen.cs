// This is the script for the "YOU DIED" message

using UnityEngine;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the text object that will fade in.")]
    private TextMeshProUGUI message;
    [SerializeField, Tooltip("How long the death message appears before loading the game over scene.")] 
    private float visibleTime;
    [SerializeField, Tooltip("How long it takes for the death message to fade in. Ideally, fadeTime <= visibleTime.")]
    private float fadeTime;

    private bool playerAlive = true;
    private float deathTime;
    private bool sceneLoaded = false;

    void Start()
    {
        // Listen to the player health OnDeath UnityEvent
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerHealth.OnDeath.AddListener(PlayerDied);
    }

    void PlayerDied()
    {
        if (playerAlive)
        {
            deathTime = Time.time;
            playerAlive = false;
        }
    }

    void Update()
    {
        if (!playerAlive)
        {
            // Fade in the death message
            message.alpha = Mathf.Min(Mathf.Max(0f, (Time.time - deathTime) / fadeTime), 1f);
            
            // Load the game over scene after visibleTime seconds
            if (Time.time > deathTime + visibleTime && !sceneLoaded)
            {
                SceneLoader.Instance.LoadSceneWithFade("GameOverScene");
                sceneLoaded = true;
            }
        }
    }
}
