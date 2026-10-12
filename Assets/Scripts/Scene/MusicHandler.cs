/*******************************************************
* Script:      MusicHandler.cs
* Author(s):   Alexander Art
* 
* Description:
*    Script to manage and play background music.
*******************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicHandler : MonoBehaviour
{
    [SerializeField, Tooltip("Volume to play the music at (from 0 to 1)"), Range(0, 1)]
    public float volume = 1.0f;
    [SerializeField, Tooltip("Time in seconds to fade out the music when a transition needs to be forced.")]
    public float fadeTime;

    public static MusicHandler instance;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private List<AudioClip> dayMusic;
    [SerializeField]
    private List<AudioClip> nightMusic;
    [SerializeField]
    private AudioClip titleScreenMusic;
    [SerializeField]
    private AudioClip gameOverMusic;

    private DayNightHandler dayNightHandler;
    private float currentFadeTime = 0.0f;

    private void Awake()
    {
        // Have only 1 MusicHandler at a time and have it persist between scenes
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        dayNightHandler = GameObject.FindAnyObjectByType<DayNightHandler>().GetComponent<DayNightHandler>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            // Stop the music if it is not playing title screen music
            if (audioSource.clip != titleScreenMusic)
            {
                audioSource.Stop();
            }

            audioSource.volume = volume;
            if (!audioSource.isPlaying)
            {
                audioSource.clip = titleScreenMusic;
                audioSource.Play();
            }
        }
        else if (SceneManager.GetActiveScene().name == "GameOverScene")
        {
            // Stop the music if it is not playing game over music
            if (audioSource.clip != gameOverMusic)
            {
                audioSource.Stop();
            }

            audioSource.volume = volume;
            if (!audioSource.isPlaying)
            {
                audioSource.clip = gameOverMusic;
                audioSource.Play();
            }
        }
        else if (dayNightHandler.IsDay())
        {
            if (nightMusic.Contains(audioSource.clip))
            {
                // If nighttime music is playing during the day, fade it out

                currentFadeTime += Time.deltaTime;
                audioSource.volume = volume * (1.0f - currentFadeTime / fadeTime);

                if (audioSource.volume <= 0.0f)
                {
                    audioSource.Stop();
                    currentFadeTime = 0.0f;
                }
            }
            else if (audioSource.clip == titleScreenMusic || audioSource.clip == gameOverMusic)
            {
                // Stop the music if it is still playing title screen or game over music in-game
                audioSource.Stop();
            }
            else
            {
                audioSource.volume = volume;
                currentFadeTime = 0.0f;
            }

            // If no music is playing
            if (!audioSource.isPlaying)
            {
                audioSource.volume = volume;
                audioSource.clip = dayMusic[(dayNightHandler.GetDayCount() - 1) % dayMusic.Count];
                audioSource.Play();
            }
        }
        else
        {
            // If daytime music is playing during the night, fade it out
            if (dayMusic.Contains(audioSource.clip))
            {
                currentFadeTime += Time.deltaTime;
                audioSource.volume = volume * (1.0f - currentFadeTime / fadeTime);

                if (audioSource.volume <= 0.0f)
                {
                    audioSource.Stop();
                    currentFadeTime = 0.0f;
                }
            }
            else if (audioSource.clip == titleScreenMusic || audioSource.clip == gameOverMusic)
            {
                // Stop the music if it is still playing title screen or game over music in-game
                audioSource.Stop();
            }
            else
            {
                audioSource.volume = volume;
                currentFadeTime = 0.0f;
            }

            // If no music is playing
            if (!audioSource.isPlaying)
            {
                audioSource.volume = volume;
                audioSource.clip = nightMusic[(dayNightHandler.GetDayCount() - 1) % nightMusic.Count];
                audioSource.Play();
            }
        }
    }
}
