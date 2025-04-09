using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mission Parameter/MissionOption")]
public class MissionOption : ScriptableObject
{
    public string parameterType; // e.g., "Mission Type"
    public string optionName;    // e.g., "Supremacy"
}
