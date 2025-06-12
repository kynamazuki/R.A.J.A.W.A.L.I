using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VSX.Utilities.UI;
using UnityEngine.EventSystems;
using VSX.UniversalVehicleCombat.Loadout;

public class MissionTutorial : MonoBehaviour
{
    public LoadoutManager loadoutManager;

    [System.Serializable]
    public class MissionButton
    {
        public string missionName;     // Label like "Gunnary"
        public string sceneName;       // Scene to load later
        public ButtonController button;          // Button to select this mission
    }

    public MissionButton[] missionButtons;
    public ButtonController startMissionButton;

    private string selectedSceneName;
    private ButtonController currentlySelectedButton;

    [SerializeField]
    protected AudioSource buttonPointerEnterAudio;

    [SerializeField]
    protected AudioSource buttonClickAudio;

    private void Start()
    {
        // Disable start button until a mission is selected
        startMissionButton.interactable = false;
        startMissionButton.onClick.AddListener(StartSelectedMission);
        AddSoundEvents(startMissionButton);

        foreach (var mission in missionButtons)
        {
            if (mission.button != null)
            {
                mission.button.onClick.AddListener(() =>
                {
                    PlayClickSound();

                    // Deselect previous
                    if (currentlySelectedButton != null)
                    {
                        currentlySelectedButton.SetSelected(false);
                        currentlySelectedButton.SetDeepSelected(false);
                    }

                    // Select new
                    mission.button.SetSelected(true);
                    mission.button.SetDeepSelected(true);
                    currentlySelectedButton = mission.button;

                    selectedSceneName = mission.sceneName;
                    startMissionButton.interactable = true;
                });

                AddSoundEvents(mission.button);
            }
        }
    }

    private void StartSelectedMission()
    {
        if (!string.IsNullOrEmpty(selectedSceneName))
        {
            loadoutManager.SavePersistentData();

            SceneManager.LoadScene(selectedSceneName);
        }
    }

    private void AddSoundEvents(Button button)
    {
        var trigger = button.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }

        var entry = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((_) => PlayHoverSound());
        trigger.triggers.Add(entry);
    }

    private void PlayHoverSound()
    {
        if (buttonPointerEnterAudio != null) buttonPointerEnterAudio.Play();
    }

    private void PlayClickSound()
    {
        if (buttonClickAudio != null) buttonClickAudio.Play();
    }
}
