using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.Utilities.UI;
using UnityEngine.EventSystems;
using VSX.UniversalVehicleCombat;

public class MissionDropDown : MonoBehaviour
{
    public ButtonController headerButton;
    public GameObject optionsContainer;
    public List<ButtonController> optionButtons;

    [SerializeField]
    protected AudioSource buttonPointerEnterAudio;

    [SerializeField]
    protected AudioSource buttonClickAudio;

    [System.Serializable]
    public class OptionSelectedEvent : UnityEvent<string> { }

    public OptionSelectedEvent onOptionSelected;

    private ButtonController currentlySelectedOption;

    // Static list to track all dropdowns
    private static List<MissionDropDown> allDropdowns = new List<MissionDropDown>();

    public enum MissionParameterType { MissionType, MissionTime, EnemyType, Ammo, Location }
    public MissionParameterType parameterType;

    public MissionParameters missionParameters;


    private void Start()
    {
        // Register this dropdown
        allDropdowns.Add(this);

        optionsContainer.SetActive(false);

        // Toggle dropdown on header click
        headerButton.onClick.AddListener(() =>
        {
            PlayClickSound();

            // Close all other dropdowns and deselect their headers
            foreach (var dropdown in allDropdowns)
            {
                if (dropdown != this)
                {
                    dropdown.CloseDropdown();
                    dropdown.headerButton.SetSelected(false); // Deselect other headers
                }
            }

            // Toggle this dropdown visibility
            bool isActive = !optionsContainer.activeSelf;
            optionsContainer.SetActive(isActive);

            // Set selected state based on visibility
            headerButton.SetSelected(isActive);
        });

        AddSoundEvents(headerButton);

        for (int i = 0; i < optionButtons.Count; i++)
        {
            int capturedIndex = i;
            var option = optionButtons[capturedIndex];
            string value = option.texts[0].text;

            AddSoundEvents(option);

            option.onClick.AddListener(() =>
            {
                PlayClickSound();

                // Deselect previous
                if (currentlySelectedOption != null)
                {
                    currentlySelectedOption.SetSelected(false);
                    currentlySelectedOption.SetDeepSelected(false);
                }

                // Select new
                option.SetSelected(true);
                option.SetDeepSelected(true);
                currentlySelectedOption = option;

                // Invoke selection logic
                onOptionSelected.Invoke(value);
                headerButton.SetText(1, value);
                optionsContainer.SetActive(false);

                // Update mission parameters based on selection
                switch (parameterType)
                {
                    case MissionParameterType.MissionType:
                        MissionParameters.Instance.missionType = value;
                        break;
                    case MissionParameterType.MissionTime:
                        SetMissionTime(value);
                        break;
                    case MissionParameterType.EnemyType:
                        MissionParameters.Instance.enemyType = value;
                        break;
                    case MissionParameterType.Ammo:
                        MissionParameters.Instance.ammo = value;
                        break;
                    case MissionParameterType.Location:
                        MissionParameters.Instance.location = value;
                        break;
                }


                onOptionSelected.Invoke(value);
            });
        }
    }

    // Sets the mission time based on selected value
    private void SetMissionTime(string value)
    {
        if (value == "Hardcore (1 Minutes)") MissionParameters.Instance.missionTime = 60f;
        else if (value == "Normal (3 Minutes)") MissionParameters.Instance.missionTime = 180f;
        else if (value == "Easy (5 Minutes)") MissionParameters.Instance.missionTime = 300f;
    }



    public void CloseDropdown()
    {
        optionsContainer.SetActive(false);
    }

    private void OnDestroy()
    {
        // Clean up
        allDropdowns.Remove(this);
    }

    private void AddSoundEvents(ButtonController button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
    }

    private void PlayHoverSound()
    {
        if (buttonPointerEnterAudio != null)
        {
            buttonPointerEnterAudio.Play();
        }
    }

    private void PlayClickSound()
    {
        if (buttonClickAudio != null)
        {
            buttonClickAudio.Play();
        }
    }
}
