using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public VisualTreeAsset MapAsset;

    public float HighDangerPropability;
    public float MediumDangerPropability;

    public float LostAtHighDangerOutcome;
    public float LostAtMediumDangerOutcome;

    public string HighOutcomeDescription;
    public string MediumOutcomeDescription;
    public string LowOutcomeDescription;
}
