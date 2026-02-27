using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    public List<SessionData> sessionHistory = new List<SessionData>();
    [HideInInspector] public SessionData currentSession;

    [Header("JSON Save for Leaderboard (optional)")]
    public string leaderboardFileName = "LeaderboardData.json";
    public bool debug = false;

    private string FilePath => Application.persistentDataPath + "/" + leaderboardFileName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLeaderboard();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -------------------
    // SESSION MANAGEMENT
    // -------------------

    public void StartNewSession(string playerName)
    {
        currentSession = new SessionData(playerName, 0);
        RefreshUI();
    }

    public void UpdateCurrentScore(int score)
    {
        if (currentSession == null) return;
        currentSession.score = score;
        RefreshUI();
    }

    public void SaveCurrentSession()
    {
        if (currentSession == null) return;

        sessionHistory.Add(currentSession);
        SaveLeaderboard();
        currentSession = null;
    }

    public void DeleteAllSessions()
    {
        sessionHistory.Clear();
        currentSession = null;

        if (File.Exists(FilePath))
            File.Delete(FilePath);

        RefreshUI();
        if (debug) Debug.Log("Deleted all leaderboard data.");
    }

    // -------------------
    // JSON PERSISTENCE
    // -------------------

    public void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(new SessionListWrapper(sessionHistory));
        File.WriteAllText(FilePath, json);

        if (debug) Debug.Log("Saved leaderboard to " + FilePath);
    }

    public void LoadLeaderboard()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            SessionListWrapper wrapper = JsonUtility.FromJson<SessionListWrapper>(json);
            sessionHistory = wrapper.sessions;
            if (debug) Debug.Log("Loaded leaderboard from " + FilePath);
        }
    }

    // -------------------
    // UI REFRESH
    // -------------------

    public void RefreshUI()
    {
        if (LeaderboardUI.Instance != null)
        {
            LeaderboardUI.Instance.Refresh();
        }
    }
}

// -------------------
// SERIALIZABLE CLASSES
// -------------------

[System.Serializable]
public class SessionListWrapper
{
    public List<SessionData> sessions;

    public SessionListWrapper(List<SessionData> list)
    {
        sessions = list;
    }
}

[System.Serializable]
public class SessionData
{
    public string playerName;
    public int score;

    public SessionData(string name, int score)
    {
        playerName = name;
        this.score = score;
    }
}