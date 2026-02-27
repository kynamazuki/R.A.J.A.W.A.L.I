using TMPro;
using UnityEngine;

public class LeaderboardRowUI : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI scoreText;

    public void Setup(string name, int score)
    {
        playerNameText.text = name;
        scoreText.text = score.ToString();
    }
}