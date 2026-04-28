using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissileGroupController : MonoBehaviour
{
    public List<GameObject> missileWeapons;

    private int currentIndex = 0;
    private GeneralInputAsset controls;
    private float scrollValue;
    private bool switchPressed;

    private void Awake()
    {
        controls = new GeneralInputAsset();

        controls.WeaponControls.ScrollMissile.performed += ctx => scrollValue = ctx.ReadValue<float>();

        //Joysticks
        controls.WeaponControls.SwitchMissile.performed += ctx => switchPressed = true;
    }

    void Start()
    {
        ActivateOnly(currentIndex);
    }

    void Update()
    {
        // Mouse scroll
        if (scrollValue > 0)
        {
            PreviousMissile();
            scrollValue = 0;
        }
        else if (scrollValue < 0)
        {
            NextMissile();
            scrollValue = 0;
        }

        //Joystick button (toggle next missile)
        if (switchPressed)
        {
            NextMissile();
            switchPressed = false;
        }
    }

    public void NextMissile()
    {
        currentIndex = (currentIndex + 1) % missileWeapons.Count;
        ActivateOnly(currentIndex);
    }

    public void PreviousMissile()
    {
        currentIndex = (currentIndex - 1 + missileWeapons.Count) % missileWeapons.Count;
        ActivateOnly(currentIndex);
    }

    void ActivateOnly(int index)
    {
        for (int i = 0; i < missileWeapons.Count; i++)
        {
            missileWeapons[i].SetActive(i == index);
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}