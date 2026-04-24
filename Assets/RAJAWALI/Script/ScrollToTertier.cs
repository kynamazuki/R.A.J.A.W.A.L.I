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

    private void Awake()
    {
        defaultControl = new GeneralInputAsset();

        // Mouse4 → CQC
        defaultControl.WeaponControls.SwitchToFirst.performed += ctx => switchToFirst = true;

        // Mouse5 → Cannon
        defaultControl.WeaponControls.SwitchToSecond.performed += ctx => switchToSecond = true;

        // Joystick toggle
        defaultControl.WeaponControls.SwitchWeapon.performed += ctx => switchPressed = true;
    }

    private void Start()
    {
        ActivateCQC(); // default weapon
    }

    private void Update()
    {
        if (switchToFirst)
        {
            ActivateCQC();
            switchToFirst = false;
        }

        if (switchToSecond)
        {
            ActivateCannon();
            switchToSecond = false;
        }

        if (switchPressed)
        {
            if (CQC_Mount.activeSelf)
                ActivateCannon();
            else
                ActivateCQC();

            switchPressed = false;
        }
    }

    // ✅ FIXED FUNCTIONS
    private void ActivateCQC()
    {
        CQC_Mount.SetActive(true);
        Cannon_Mount.SetActive(false);

    }

    private void ActivateCannon()
    {
        CQC_Mount.SetActive(false);
        Cannon_Mount.SetActive(true);


    }

    private void OnEnable() => defaultControl.Enable();
    private void OnDisable() => defaultControl.Disable();
}