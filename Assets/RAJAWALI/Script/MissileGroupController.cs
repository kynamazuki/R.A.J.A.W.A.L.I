using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissileGroupController : MonoBehaviour
{
    [Header("Actual Missile Weapon Objects")]
    public List<GameObject> missileWeapons;

    [Header("Visible Missile Handle Rows")]
    public List<RectTransform> missileUIRows;

    private List<Vector2> originalPositions = new List<Vector2>();

    private int currentIndex = 0;
    private GeneralInputAsset controls;
    private float scrollValue;
    private bool switchPressed;

    private void Awake()
    {
        controls = new GeneralInputAsset();

        controls.WeaponControls.ScrollMissile.performed += ctx => scrollValue = ctx.ReadValue<float>();
        controls.WeaponControls.SwitchMissile.performed += ctx => switchPressed = true;
    }

    void Start()
    {
        for (int i = 0; i < missileUIRows.Count; i++)
        {
            originalPositions.Add(missileUIRows[i].anchoredPosition);
        }

        UpdateMissileSelection();
    }

    void Update()
    {
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

        if (switchPressed)
        {
            NextMissile();
            switchPressed = false;
        }
    }

    public void NextMissile()
    {
        currentIndex = (currentIndex + 1) % missileWeapons.Count;
        UpdateMissileSelection();
    }

    public void PreviousMissile()
    {
        currentIndex = (currentIndex - 1 + missileWeapons.Count) % missileWeapons.Count;
        UpdateMissileSelection();
    }

    void UpdateMissileSelection()
    {
        // Activate only selected missile weapon
        for (int i = 0; i < missileWeapons.Count; i++)
        {
            missileWeapons[i].SetActive(i == currentIndex);
        }

        // Move visible UI rows only
        for (int slot = 0; slot < missileUIRows.Count; slot++)
        {
            int rowIndex = (currentIndex + slot) % missileUIRows.Count;
            missileUIRows[rowIndex].anchoredPosition = originalPositions[slot];
        }
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}