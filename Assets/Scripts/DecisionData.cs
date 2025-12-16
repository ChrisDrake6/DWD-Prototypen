using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DecisionData", menuName = "Scriptable Objects/DecisionData")]
public class DecisionData : ScriptableObject
{
    public DangerLevel DangerLevel;
    public DecisionDataEntry[] Contents;
    public float Cost;
    public float LostReduction;
}

[Serializable]
public class DecisionDataEntry
{
    public string ActionDescription;
    public string LowDangerOutcome;
    public string MediumDangerOutcome;
    public string HighDangerOutcome;
}
