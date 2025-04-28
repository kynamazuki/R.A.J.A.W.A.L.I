using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScrollToTertier : MonoBehaviour
{
    public GameObject FirstWeapon;
    public GameObject SecondWeapon;

    private GeneralInputAsset defaultControl;
    private float mouseScrollY;

    private void Awake()
    {
        defaultControl = new GeneralInputAsset();
        defaultControl.WeaponControls.ScrollToTertier.performed += ctx => mouseScrollY = ctx.ReadValue<float>();
    }

    private void Update()
    {
        if (mouseScrollY < 0) // Scroll Down
        {
            // Swap positions of the first and second weapon
            Vector3 tempPosition = FirstWeapon.transform.position;
            FirstWeapon.transform.position = SecondWeapon.transform.position;
            SecondWeapon.transform.position = tempPosition;

            mouseScrollY = 0; // Reset to prevent multiple swaps
        }
        else if (mouseScrollY > 0) // Scroll Up
        {
            // Swap positions of the first and second weapon
            Vector3 tempPosition = FirstWeapon.transform.position;
            FirstWeapon.transform.position = SecondWeapon.transform.position;
            SecondWeapon.transform.position = tempPosition;

            mouseScrollY = 0; // Reset to prevent multiple swaps
        }
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
