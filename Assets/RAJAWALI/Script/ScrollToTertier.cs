using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScrollToTertier : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject FirstWeapon;
    public GameObject SecondWeapon;

    private GeneralInputAsset defaultControl;
    private float mouseScrollY;

    // Tracks whether the joystick button was pressed
    private bool switchPressed;

    private void Awake()
    {
        SecondWeapon.SetActive(false); // Ensure the secondary weapon is inactive initially

        defaultControl = new GeneralInputAsset();

        // Scroll wheel input
        defaultControl.WeaponControls.ScrollToTertier.performed += ctx => mouseScrollY = ctx.ReadValue<float>();

        // Joystick button input
        defaultControl.WeaponControls.SwitchWeapon.performed += ctx => switchPressed = true;
    }

    private void Update()
    {
        // Scroll down ? switch to second weapon
        if (mouseScrollY < 0)
        {
            SwitchToSecondWeapon();
            mouseScrollY = 0;
        }
        // Scroll up ? switch back to first weapon
        else if (mouseScrollY > 0)
        {
            SwitchToFirstWeapon();
            mouseScrollY = 0;
        }

        // Joystick button pressed ? toggle weapons
        if (switchPressed)
        {
            if (FirstWeapon.activeSelf)
                SwitchToSecondWeapon();
            else
                SwitchToFirstWeapon();

            switchPressed = false;
        }
    }

    private void SwitchToFirstWeapon()
    {
        FirstWeapon.SetActive(true);
        SecondWeapon.SetActive(false);
    }

    private void SwitchToSecondWeapon()
    {
        FirstWeapon.SetActive(false);
        SecondWeapon.SetActive(true);
    }

    private void OnEnable()
    {
        defaultControl.Enable();
    }

    private void OnDisable()
    {
        defaultControl.Disable();
    }
}
