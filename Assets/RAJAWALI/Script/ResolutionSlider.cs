using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ResolutionSlider : MonoBehaviour
{
    public Slider resolutionSlider;
    public TMP_Text resolutionLabel;

    Coroutine hideCoroutine;

    void Start()
    {
        int savedValue = PlayerPrefs.GetInt("ResolutionIndex", 0);

        resolutionSlider.value = savedValue;
        ApplyResolution(savedValue);

        resolutionLabel.gameObject.SetActive(false);
    }

    public void OnResolutionChanged(float value)
    {
        Debug.Log("Slider raw value: " + value);

        int index = Mathf.RoundToInt(value);
        Debug.Log("Resolution index: " + index);

        ApplyResolution(index);

        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    void ApplyResolution(int index)
    {
        int width = 1920;
        int height = 1080;

        switch (index)
        {
            case 0:
                width = 1920;
                height = 1080;
                break;
            case 1:
                width = 2560;
                height = 1440;
                break;
            case 2:
                width = 3840;
                height = 2160;
                break;
        }

        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.SetResolution(width, height, true);

        StartCoroutine(ConfirmResolution());
        ShowLabel(index);
    }


    void ShowLabel(int index)
    {
        resolutionLabel.gameObject.SetActive(true);

        switch (index)
        {
            case 0:
                resolutionLabel.text = "1920 × 1080";
                break;
            case 1:
                resolutionLabel.text = "2560 × 1440";
                break;
            case 2:
                resolutionLabel.text = "3840 × 2160";
                break;
        }

        // Reset hide timer
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        resolutionLabel.gameObject.SetActive(false);
    }

    IEnumerator ConfirmResolution()
    {
        yield return null; // wait 1 frame
        Debug.Log($"Confirmed: {Screen.width} x {Screen.height}");
    }
}
