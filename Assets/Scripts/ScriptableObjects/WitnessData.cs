using System;
using UnityEngine;

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
