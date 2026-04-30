using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VSX.UniversalVehicleCombat;

public class ScrollToTertier : MonoBehaviour
{
    [Header("Weapon Mounts")]
    public GameObject CQC_Mount;
    public GameObject Cannon_Mount;

    public TriggerablesManager triggerablesManager;

    private GeneralInputAsset defaultControl;
    private bool switchToFirst;
    private bool switchToSecond;
    private bool switchPressed;

    private bool usingCQC = true;   // ✅ track current mode ourselves

    private void Awake()
    {
        defaultControl = new GeneralInputAsset();

        // Mouse4 → CQC
        defaultControl.WeaponControls.SwitchToFirst.performed += ctx => switchToFirst = true;

        // Mouse5 → Cannon
        defaultControl.WeaponControls.SwitchToSecond.performed += ctx => switchToSecond = true;

        // G key + joystick toggle
        defaultControl.WeaponControls.SwitchWeapon.performed += ctx => switchPressed = true;
    }

    private void Start()
    {
        ActivateCQC();
        switchPressed = false;
        switchToFirst = false;
        switchToSecond = false;
    }

    private void Update()
    {
        if (switchToFirst)
        {
            ActivateCQC();
            switchToFirst = false;
        }
        else if (switchToSecond)
        {
            ActivateCannon();
            switchToSecond = false;
        }
        else if (switchPressed)
        {
            if (usingCQC)
                ActivateCannon();
            else
                ActivateCQC();

            switchPressed = false;
        }
    }

    private void ActivateCQC()
    {
        CQC_Mount.SetActive(true);
        Cannon_Mount.SetActive(false);

        usingCQC = true;
    }

    private void ActivateCannon()
    {
        CQC_Mount.SetActive(false);
        Cannon_Mount.SetActive(true);

        usingCQC = false;
    }

    private void OnEnable() => defaultControl.Enable();
    private void OnDisable() => defaultControl.Disable();
}