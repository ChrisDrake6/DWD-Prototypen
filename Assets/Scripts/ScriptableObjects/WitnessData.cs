using System;
using UnityEngine;

/// <summary>
/// Contains all data of the witnesses to display for given DangerLevel. Needs to be referenced to the GameManager.
/// </summary>
[CreateAssetMenu(fileName = "ConsequencePreview", menuName = "Scriptable Objects/ConsequencePreview")]
public class WitnessData : ScriptableObject
{
    public DangerLevel DangerLevel;
    public WitnessDataEntry[] entries;
}

[Serializable]
public class WitnessDataEntry
{
    public Sprite ProfileImage;
    [TextArea(10, 15)]
    public string Text;
}
