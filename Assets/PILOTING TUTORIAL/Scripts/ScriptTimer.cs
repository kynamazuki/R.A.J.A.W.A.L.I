using UnityEngine;
using UnityEngine.UI;

public class ScriptTimer : MonoBehaviour
{
    Text text;
    public static float waktu = 300f; // 3minit

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        waktu -= Time.deltaTime;
        if (waktu < 0)
            waktu = 0;

        int minit = Mathf.FloorToInt(waktu / 60);
        int saat = Mathf.FloorToInt(waktu % 60);
        text.text = "⏳ " + minit.ToString("00") + ":" + saat.ToString("00");
    }
}