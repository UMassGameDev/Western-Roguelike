/*******************************************************
* Script:      Score.cs
* Author(s):   Alexander Art
* 
* Description:
*    Keep track of and display the player's score
*******************************************************/

using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    // This is where the score is stored
    private int score;

    // Reference to the score text so it is kept updated
    [SerializeField] private TextMeshProUGUI scoreDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the score
        SetScore(0);
    }

    //~(GetScore)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Return the player's score
    public int GetScore()
    {
        return score;
    }

    //~(AddScore)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Adds to the player's score. (To subtract, use a negative number)
    public void AddScore(int increment)
    {
        score += increment;

        // Prevent negative scores
        if (score < 0)
        {
            score = 0;
        }

        // Update the display
        scoreDisplay.text = score.ToString("D6");
    }

    //~(SetScore)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Sets the player's score
    public void SetScore(int newScore)
    {
        score = newScore;

        // Prevent negative scores
        if (score < 0)
        {
            score = 0;
        }

        // Update the display
        scoreDisplay.text = score.ToString("D6");
    }
}
