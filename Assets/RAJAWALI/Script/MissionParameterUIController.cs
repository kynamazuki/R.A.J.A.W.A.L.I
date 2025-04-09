using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionParameterUIController : MonoBehaviour
{
    [System.Serializable]
    public class MissionParameterGroup
    {
        public string title; // e.g., "Mission Type"
        public List<string> options; // e.g., Supremacy, Strike, Defend
    }

    public Transform parameterListParent; // Parent for category buttons
    public Transform optionListParent; // Parent for option buttons
    public GameObject categoryButtonPrefab;
    public GameObject optionButtonPrefab;

    public List<MissionParameterGroup> missionParameters;

    void Start()
    {
        PopulateParameterCategories();
    }

    void PopulateParameterCategories()
    {
        foreach (var group in missionParameters)
        {
            GameObject categoryGO = Instantiate(categoryButtonPrefab, parameterListParent);
            categoryGO.GetComponentInChildren<Text>().text = group.title;

            Button btn = categoryGO.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                ShowOptions(group.options);
            });
        }
    }

    void ShowOptions(List<string> options)
    {
        // Clear previous options
        foreach (Transform child in optionListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var option in options)
        {
            GameObject optionGO = Instantiate(optionButtonPrefab, optionListParent);
            optionGO.GetComponentInChildren<Text>().text = option;

            Button btn = optionGO.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                Debug.Log("Selected: " + option);
                // Store selected option here if needed
            });
        }
    }
}
