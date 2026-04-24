using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VSX.UniversalVehicleCombat.Loadout;

public class LeaderboardUI : MonoBehaviour
{
    public static LeaderboardUI Instance;

    [Header("UI")]
    public GameObject panel;
    public Transform contentParent;   // The parent content panel for the rows
    public GameObject rowPrefab;      // Prefab with LeaderboardRowUI component

    [SerializeField] private string loadoutSceneName = "LoadoutArcade";

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Refresh();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        panel.SetActive(false);
    }

    public void HideLeaderboard()
    {
        panel.SetActive(false);
    }

    public void ShowLeaderboard()
    {
        panel.SetActive(true);
        Refresh();
    }


    /// <summary>
    /// Refresh the leaderboard table: clear old rows, instantiate new ones
    /// </summary>
    public void Refresh()
    {
        if (LeaderboardManager.Instance == null) return;

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        List<SessionData> displayList = new List<SessionData>();

        // Add valid past sessions
        foreach (var session in LeaderboardManager.Instance.sessionHistory)
        {
            if (!string.IsNullOrEmpty(session.playerName))
            {
                displayList.Add(session);
            }
        }

        // SORT (highest first)
        displayList.Sort((a, b) => b.score.CompareTo(a.score));

        // LIMIT TOP 10
        if (displayList.Count > 10)
        {
            displayList = displayList.GetRange(0, 10);
        }

        // Spawn rows
        for (int i = 0; i < displayList.Count; i++)
        {
            GameObject rowObj = Instantiate(rowPrefab, contentParent);
            var rowUI = rowObj.GetComponent<LeaderboardRowUI>();

            rowUI.Setup(displayList[i].playerName, displayList[i].score);
        }

        Debug.Log("Showing Top: " + displayList.Count);
    }

    public void PlayAgain()
    {
        Debug.Log("Play Again clicked");

        // ✅ Use Instance directly
        if (LoadoutManager.Instance != null)
        {
            LoadoutManager.Instance.LoadoutData.currentMissionIndex = 0;
            LoadoutManager.Instance.SavePersistentData();
        }

        // Reset score
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.currentScore = 0;
        }

        HideLeaderboard();
        PlayerProfileUI.Instance.HideNameInput();

        SceneManager.LoadScene(loadoutSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit clicked");
        Application.Quit();
    }
}