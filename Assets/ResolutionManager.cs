using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    void Awake()
    {
        bool supported = false;
        foreach (Resolution res in Screen.resolutions)
        {
            if (res.width == 5760 && res.height == 1080)
            {
                supported = true;
                break;
            }
        }

        if (supported)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.SetResolution(5760, 1080, true);
        }
        else
        {
            Debug.Log("5760x1080 not supported. Using default resolution.");
        }
    }

    void Start()
    {
        Debug.Log($"Resolution applied: {Screen.width}x{Screen.height} | Fullscreen: {Screen.fullScreen}");
    }

}
