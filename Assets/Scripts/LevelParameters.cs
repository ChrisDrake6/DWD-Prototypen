using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "LevelParameters", menuName = "Scriptable Objects/LevelData")]
public class LevelParameters : ScriptableObject
{
    public VisualTreeAsset MapAsset;

    [Header("Propabilities")]
    public float HighDangerPropability;
    public float MediumDangerPropability;


    [Header("Genereal Outcome Descriptions")]
    public string HighOutcomeDescription;
    public string MediumOutcomeDescription;
    public string LowOutcomeDescription;

    [Header("Transition Text Lines")]
    [TextArea(10, 15)]
    public string LowOutComeLowDecisionTransitionText;
    [TextArea(10, 15)]
    public string LowOutComeMediumDecisionTransitionText;
    [TextArea(10, 15)]
    public string LowOutComeHighDecisionTransitionText;

    [TextArea(10, 15)]
    public string MediumOutComeLowDecisionTransitionText;
    [TextArea(10, 15)]
    public string MediumOutComeMediumDecisionTransitionText;
    [TextArea(10, 15)]
    public string MediumOutComeHighDecisionTransitionText;

    [TextArea(10, 15)]
    public string HighOutComeLowDecisionTransitionText;
    [TextArea(10, 15)]
    public string HighOutComeMediumDecisionTransitionText;
    [TextArea(10, 15)]
    public string HighOutComeHighDecisionTransitionText;
}
