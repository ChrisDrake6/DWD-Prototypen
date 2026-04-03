using System;
using UnityEngine;

/// <summary>
/// Contains the text for the decision package corresponding to the given DangerLevel, as well as the outcome texts. Needs to be referenced to the GameManager.
/// </summary>
[CreateAssetMenu(fileName = "DecisionData", menuName = "Scriptable Objects/DecisionData")]
public class DecisionData : ScriptableObject
{
    public DangerLevel DangerLevel;
    [TextArea(10, 15)]
    public string ActionDescription;
    [TextArea(10, 15)]
    public string LowDangerOutcome;
    [TextArea(10, 15)]
    public string MediumDangerOutcome;
    [TextArea(10, 15)]
    public string HighDangerOutcome;
}
