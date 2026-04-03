using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<LevelContentContainer> RoundStarted;
    public static event Action<List<OutcomeData>> LastDecisionMade;
    public static event Action<OutcomeData> OutcomeCalculated;

    [SerializeField] private LevelContentContainer[] levelContentContainers;

    private int _currentRound = -1;
    private List<OutcomeData> _outcomes = new List<OutcomeData>();

    void Start()
    {
        DecisionsUIManager.DecisionMade += OnDecisionMade;
        HelperUIManager.OnBoardingDone += StartNewRound;
        TransitionUIManager.TransitionClosed += StartNewRound;
    }

    private void OnDestroy()
    {
        DecisionsUIManager.DecisionMade -= OnDecisionMade;
        HelperUIManager.OnBoardingDone -= StartNewRound;
        TransitionUIManager.TransitionClosed -= StartNewRound;
    }

    /// <summary>
    /// Checks if there is another round left, starts next round if there is, triggers evaluation if not.
    /// Gets triggered on completion of the onboarding segment or on continuing from the transition page.
    /// </summary>
    private void StartNewRound()
    {
        if (_currentRound < levelContentContainers.Length - 1)
        {
            _currentRound++;
            RoundStarted.Invoke(levelContentContainers[_currentRound]);
        }
        else
        {
            LastDecisionMade.Invoke(_outcomes);
        }
    }


    /// <summary>
    /// Gets called once the player chooses a decision package.
    /// </summary>
    /// <param name="dangerLevel"></param>
    private void OnDecisionMade(DangerLevel dangerLevel)
    {
        // Calculate outcome: Imagine a scale from 0 to 100. Each Dangerlevel gets a distinct range on this scale, in the amount of its propability.
        // For example: High danger has 10 % propabality of happening, medium has 40, low has 50. Roll a D100 and check the value on the scale. If the calue of the die is below 40, it is a medium Outcome.
        // If it is between 40 and 50, it is a high outcome. If it is neither, its low by default.
        DangerLevel outcome = DangerLevel.low;
        Dictionary<DangerLevel, float> propabilities = new Dictionary<DangerLevel, float>()
        {
            { DangerLevel.medium, levelContentContainers[_currentRound].LevelParameters.MediumDangerPropability},
            { DangerLevel.high, levelContentContainers[_currentRound].LevelParameters.HighDangerPropability}
        };
        float d100 = UnityEngine.Random.Range(0, 100);
        float rangeOffset = 0;
        foreach (KeyValuePair<DangerLevel, float> prop in propabilities)
        {
            if (prop.Value + rangeOffset >= d100)
            {
                outcome = prop.Key;
                break;
            }
            rangeOffset += prop.Value;
        }

        // Build data structure for evaluation.
        OutcomeData outcomeData = new OutcomeData()
        {
            Outcome = outcome,
            Decision = levelContentContainers[_currentRound].Decisions.FirstOrDefault(a => a.DangerLevel == dangerLevel),
            Level = levelContentContainers[_currentRound].LevelParameters
        };
        _outcomes.Add(outcomeData);

        OutcomeCalculated.Invoke(outcomeData);
    }
}
