using UnityEngine;
using VSX.CameraSystem;
using VSX.UniversalVehicleCombat;

public class CameraViewChangeNotifier : MonoBehaviour
{
    public VehicleCamera vehicleCamera;

    private CameraView lastView;

    private void Update()
    {
        if (vehicleCamera != null && vehicleCamera.CurrentView != lastView)
        {
            lastView = vehicleCamera.CurrentView;
            vehicleCamera.NotifyCameraViewChanged(lastView);
        }
    }
}
