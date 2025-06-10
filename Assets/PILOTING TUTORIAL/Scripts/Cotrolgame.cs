using UnityEngine;
using UnityEngine.SceneManagement;

public class CotrolGame : MonoBehaviour
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
        ScriptTimer.waktu = 300f; // Reset 5 minit
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}