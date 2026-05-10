using UnityEngine;
using VSX.UniversalVehicleCombat;

public class AutoScreenLayoutManager : MonoBehaviour
{
    [Header("Canvas Objects")]
    public GameObject canvasNormalObject;
    public GameObject canvasTripleObject;

    [Header("HUD Components - Optional")]
    public HUDComponent canvasNormalHUD;
    public HUDComponent canvasTripleHUD;

    void Start()
    {
        DetectScreenMode();
    }

    void DetectScreenMode()
    {
        int w = Screen.width;
        int h = Screen.height;

        Debug.Log("Detected Resolution: " + w + " x " + h);

        bool isTripleMonitor = w >= 5000;

        if (canvasNormalObject != null)
            canvasNormalObject.SetActive(!isTripleMonitor);

        if (canvasTripleObject != null)
            canvasTripleObject.SetActive(isTripleMonitor);

        // Only use Unplugged if this canvas has HUDComponent
        if (canvasNormalHUD != null)
            canvasNormalHUD.Unplugged = isTripleMonitor;

        if (canvasTripleHUD != null)
            canvasTripleHUD.Unplugged = !isTripleMonitor;

        Debug.Log(isTripleMonitor ? "Triple Monitor UI Activated" : "Normal UI Activated");
    }
}