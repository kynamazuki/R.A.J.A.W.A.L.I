using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ScriptTimer : MonoBehaviour
{
    public TextMeshProUGUI text;
    public static float timer = 240f; // 4 minutes
    private bool gameOverTriggered = false;

    [Header("Game Over Event")]
    public UnityEvent onGameOver;

    void Start()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0 && !gameOverTriggered)
            {
                timer = 0;
                gameOverTriggered = true;
                onGameOver.Invoke();
            }

            int minit = Mathf.FloorToInt(timer / 60);
            int saat = Mathf.FloorToInt(timer % 60);
            text.text = " " + minit.ToString("00") + ":" + saat.ToString("00");
        }
    }
}
