using UnityEngine;
using TMPro;

public class ScoreHUD : MonoBehaviour
{
    public static ScoreHUD Instance;

    [SerializeField] private TMP_Text scoreText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateScoreDisplay(int score)
    {

        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}