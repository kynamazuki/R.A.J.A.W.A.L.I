using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.Utilities.UI;
using UnityEngine.EventSystems;

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

                switch (parameterType)
                {
                    case MissionParameterType.MissionType:
                        missionParameters.missionType = value;
                        break;
                    case MissionParameterType.MissionTime:
                        if (value == "Hardcore") missionParameters.missionTime = 60f;
                        else if (value == "Normal") missionParameters.missionTime = 180f;
                        else if (value == "Easy") missionParameters.missionTime = 300f;
                        break;
                    case MissionParameterType.EnemyType:
                        missionParameters.enemyType = value;
                        break;
                    case MissionParameterType.Ammo:
                        missionParameters.ammoType = value;
                        break;
                    case MissionParameterType.Location:
                        missionParameters.location = value;
                        break;
                }

                onOptionSelected.Invoke(value);
            });
       
        }
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
