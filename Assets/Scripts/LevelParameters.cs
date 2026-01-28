using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "LevelParameters", menuName = "Scriptable Objects/LevelData")]
public class LevelParameters : ScriptableObject
{
    public VisualTreeAsset MapAsset;

    [Header("Propabilities")]
    public float HighDangerPropability;
    public float MediumDangerPropability;

    [Header("Expected Lost Values")]
    public float LostAtHighDangerOutcome;
    public float LostAtMediumDangerOutcome;

    [Header("Genereal Outcome Descriptions")]
    public string HighOutcomeDescription;
    public string MediumOutcomeDescription;
    public string LowOutcomeDescription;

    [Header("Transition Text Lines")]
    public string LowOutComeLowDecisionTransitionText;
    public string LowOutComeMediumDecisionTransitionText;
    public string LowOutComeHighDecisionTransitionText;

    public string MediumOutComeLowDecisionTransitionText;
    public string MediumOutComeMediumDecisionTransitionText;
    public string MediumOutComeHighDecisionTransitionText;

    public string HighOutComeLowDecisionTransitionText;
    public string HighOutComeMediumDecisionTransitionText;
    public string HighOutComeHighDecisionTransitionText;
}
