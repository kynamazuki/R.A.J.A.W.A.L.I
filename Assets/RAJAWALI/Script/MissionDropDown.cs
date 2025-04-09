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

    private ButtonController currentSelectedOption;

    private void Start()
    {
        optionsContainer.SetActive(false);

        // Header button click sound
        headerButton.onClick.AddListener(() =>
        {
            PlayClickSound();
            optionsContainer.SetActive(!optionsContainer.activeSelf);
        });

        AddSoundEvents(headerButton);

        foreach (var option in optionButtons)
        {
            var thisOption = option; // Proper closure
            var optionValue = thisOption.texts[0].text;

            thisOption.onClick.AddListener(() =>
            {
                PlayClickSound();

                // Deselect previous selected option
                if (currentSelectedOption != null)
                {
                    currentSelectedOption.SetSelected(false);
                    currentSelectedOption.SetDeepSelected(false);
                }

                // Select this option
                thisOption.Select();
                thisOption.SetDeepSelected(true);
                currentSelectedOption = thisOption;

                // Update header text and notify listeners
                headerButton.SetText(1, optionValue);
                onOptionSelected.Invoke(optionValue);

                optionsContainer.SetActive(false);
            });

            AddSoundEvents(thisOption);
        }
    }

    private void AddSoundEvents(ButtonController button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
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
