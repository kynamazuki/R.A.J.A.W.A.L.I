using System.Collections.Generic;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    public static LeaderboardUI Instance;

    [Header("UI")]
    public Transform contentParent;   // The parent content panel for the rows
    public GameObject rowPrefab;      // Prefab with LeaderboardRowUI component

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    /// <summary>
    /// Refresh the leaderboard table: clear old rows, instantiate new ones
    /// </summary>
    public void Refresh()
    {
        if (LeaderboardManager.Instance == null) return;

        // 1️⃣ Clear old rows
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 2️⃣ Prepare list to display
        List<SessionData> displayList = new List<SessionData>();

        // Add past sessions
        displayList.AddRange(LeaderboardManager.Instance.sessionHistory);

        // Add current live session at the top if it exists
        if (LeaderboardManager.Instance.currentSession != null)
        {
            displayList.Insert(0, LeaderboardManager.Instance.currentSession);
        }

        // 3️⃣ Instantiate rows
        for (int i = 0; i < displayList.Count; i++)
        {
            GameObject rowObj = Instantiate(rowPrefab, contentParent);
            LeaderboardRowUI rowUI = rowObj.GetComponent<LeaderboardRowUI>();
            if (rowUI != null)
            {
                rowUI.Setup(displayList[i].playerName, displayList[i].score);
            }
            else
            {
                Debug.LogError("LeaderboardRowUI component missing on rowPrefab!");
            }
        }
    }
}