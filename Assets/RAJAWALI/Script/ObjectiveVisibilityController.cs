using UnityEngine;
using VSX.CameraSystem;
using VSX.UniversalVehicleCombat;

public class ObjectiveVisibilityController : MonoBehaviour
{
    public VehicleCamera vehicleCamera;
    public GameObject objectiveUI;
    public CameraView firstPersonView;

    private void Start()
    {
        if (vehicleCamera != null)
        {
            vehicleCamera.onCameraViewChanged.AddListener(OnCameraViewChanged);
        }

        if (objectiveUI != null)
        {
            objectiveUI.SetActive(false);
        }
    }

    private void OnCameraViewChanged(CameraView view)
    {
        //Debug.Log("Camera View Changed to: " + view.ID);

        if (view == firstPersonView)
        {
            objectiveUI.SetActive(true);
        }
        else
        {
            objectiveUI.SetActive(false);
        }
    }
}
