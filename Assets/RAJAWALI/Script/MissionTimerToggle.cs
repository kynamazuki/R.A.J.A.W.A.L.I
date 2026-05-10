using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionTimerToggle : MonoBehaviour
{
    public Toggle timerToggle;
    public TMP_Text statusText;

    private void Start()
    {
        timerToggle.isOn = MissionParameters.Instance.timerEnabled;

        UpdateVisual(timerToggle.isOn);

        timerToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        MissionParameters.Instance.timerEnabled = isOn;

        UpdateVisual(isOn);
    }

    private void UpdateVisual(bool isOn)
    {
        if (isOn)
        {
            statusText.text = "5 Minutes";
            MissionParameters.Instance.missionTime = 300f;
        }
        else
        {
            statusText.text = "5 Minutes";
            MissionParameters.Instance.missionTime = -1f;
        }
    }
}