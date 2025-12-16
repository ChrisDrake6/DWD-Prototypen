using UnityEngine;

[CreateAssetMenu(fileName = "SuggestionData", menuName = "Scriptable Objects/SuggestionData")]
public class SuggestionData : ScriptableObject
{
    public Sprite[] Maps;
    public float DangerPropability;
    public string SuggestionDescription;
    public string Explanation;
    public float NegativeHappinessImpact;
    public float PositiveHappinessImpact;
    public float UnneccessaryActionHappinessImpact;
    public float Cost;
    public float Lost;
}
