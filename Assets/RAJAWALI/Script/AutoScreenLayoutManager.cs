using UnityEngine;

public class AutoScreenLayoutManager : MonoBehaviour
{
    public GameObject canvasNormal;
    public GameObject canvasTriple;

    void Start()
    {
        DetectScreenMode();
    }

    void DetectScreenMode()
    {
        int w = Screen.width;
        int h = Screen.height;

        Debug.Log("Detected Resolution: " + w + " x " + h);

        if (w >= 5000) // surround monitor
        {
            canvasNormal.SetActive(false);
            canvasTriple.SetActive(true);

            Debug.Log("Triple Monitor UI Activated");
        }
        else
        {
            canvasNormal.SetActive(true);
            canvasTriple.SetActive(false);

            Debug.Log("Normal UI Activated");
        }
    }
}