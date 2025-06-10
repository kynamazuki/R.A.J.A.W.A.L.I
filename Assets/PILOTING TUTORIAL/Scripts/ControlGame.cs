using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlGame : MonoBehaviour
{
    public GameObject waktuHabisPanel, ulangiButton;

    void Update()
    {
        if (ScriptTimer.waktu <= 0)
        {
            Time.timeScale = 0;
            waktuHabisPanel.SetActive(true);
            ulangiButton.SetActive(true);
        }
    }

    public void PlayAgain()
    {
        waktuHabisPanel.SetActive(false);
        ulangiButton.SetActive(false);
        Time.timeScale = 1;
        ScriptTimer.waktu = 240f; // Reset 4 minit
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}