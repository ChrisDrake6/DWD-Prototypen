using System;
using UnityEngine;

/// <summary>
/// Contains all Helper texts and images.
/// </summary>
[CreateAssetMenu(fileName = "HelperData", menuName = "Scriptable Objects/HelperData")]
public class HelperData : ScriptableObject
{
    public HelperDataEntry[] OnBoardingLines;
    public HelperDataEntry[] WeatherMapLines;
    public HelperDataEntry[] DangerPreviewLines;
    public HelperDataEntry[] DecisionLines;
    public HelperDataEntry[] TransitionLines;
    public HelperDataEntry[] OutcomeLines;
    public HelperDataEntry[] FinalScreenLines;
}

[Serializable]
public class HelperDataEntry
{
    [TextArea(10, 15)]
    public string Text;
    public Sprite Image;
}
