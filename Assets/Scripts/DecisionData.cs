using System;
using UnityEngine;

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
