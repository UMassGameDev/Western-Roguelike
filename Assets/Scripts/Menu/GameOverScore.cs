using UnityEngine;
using TMPro;

public class GameOverScore : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the text object that will display the score.")]
    private TextMeshProUGUI scoreDisplay;

    void Start()
    {
        // Display the score
        scoreDisplay.text = "SCORE: " + Score.tempScore.ToString("D6");
    }
}
