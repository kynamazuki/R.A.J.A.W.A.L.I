using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeaderboardUI.Instance.HideLeaderboard();
        PlayerProfileUI.Instance.HideNameInput();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
