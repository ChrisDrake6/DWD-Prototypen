using UnityEngine;

public class SuggestionResultData
{
    public float CostSum { get; set; }
    public float LostSum { get; set; }
    public float HappinessLevel { get; set; }

    public SuggestionResultData()
    {
        HappinessLevel = 100;
    }
}
