using UnityEngine;
using TMPro;
using VSX.UniversalVehicleCombat.Loadout;
using System.Collections;
using System.Collections.Generic;

public class PlayerProfileUI : MonoBehaviour
{
    public static PlayerProfileUI Instance;

    public TMP_InputField nameInput;
    public GameObject namePanel;
    public LoadoutManager loadoutManager;

    public CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 3f;

    private void Awake()
    {
        Instance = this;
        namePanel.SetActive(false); // hide at start
    }

    public void ShowNamePanel()
    {
        namePanel.SetActive(true);
        nameInput.text = "";
        nameInput.ActivateInputField();
    }

    public void OnEnterPressed()
    {
        Debug.Log("Manager Instance ID: " + LeaderboardManager.Instance.GetInstanceID());
        string playerName = nameInput.text;

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player";
        }

        int finalScore = LeaderboardManager.Instance.currentScore;

        Debug.Log("ENTER PRESSED: " + nameInput.text);

        // Proper flow
        LeaderboardManager.Instance.StartNewSession(playerName);
        LeaderboardManager.Instance.UpdateCurrentScore(finalScore);
        LeaderboardManager.Instance.SaveCurrentSession();

        Debug.Log("After Save Count: " + LeaderboardManager.Instance.sessionHistory.Count);

        // Refresh UI
        if (LeaderboardUI.Instance != null)
        {
            LeaderboardUI.Instance.ShowLeaderboard();
            LeaderboardUI.Instance.Refresh();
        }

        //namePanel.SetActive(false);
    }

    public void HideNameInput()
    {
        namePanel.SetActive(false);
    }

    public void ShowAfterDeath()
    {
        StartCoroutine(ShowUIRoutine());
    }

    private IEnumerator ShowUIRoutine()
    {
        yield return null; // wait 1 frame

        namePanel.SetActive(true);

        if (LeaderboardUI.Instance != null)
            LeaderboardUI.Instance.ShowLeaderboard();

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}