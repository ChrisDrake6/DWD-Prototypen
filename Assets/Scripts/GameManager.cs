using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<LevelContentContainer> RoundStarted;
    public static event Action<List<OutcomeData>> LastDecisionMade;
    public static event Action<OutcomeData> OutcomeCalculated;

    private string _path;
    private string[] directories;
    private int _currentRound;

    private LevelContentContainer _levelContentContainer = new LevelContentContainer();

    private List<OutcomeData> _outcomes = new List<OutcomeData>();

    void Start()
    {
        _path = Path.Combine("Assets", "Levels");
        directories = Directory.GetDirectories(_path);
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

    private void StartNewRound()
    {
        if (_currentRound < directories.Length)
        {
            // Get Files and distribute Data.
            _levelContentContainer.LevelParameters = CustomUtils.GetSOAssets<LevelParameters>(directories[_currentRound])[0];
            _levelContentContainer.Consequences = CustomUtils.GetSOAssets<ConsequencePreview>(Path.Combine(directories[_currentRound], "Consequences"));
            _levelContentContainer.Decisions = CustomUtils.GetSOAssets<DecisionData>(Path.Combine(directories[_currentRound], "Decisions"));

            RoundStarted.Invoke(_levelContentContainer);

            _currentRound++;
        }
        else
        {
            LastDecisionMade.Invoke(_outcomes);
        }
    }

    private void OnDecisionMade(DangerLevel dangerLevel)
    {
        // Calculate outcome
        DangerLevel outcome = DangerLevel.low;
        Dictionary<DangerLevel, float> propabilities = new Dictionary<DangerLevel, float>()
        {
            { DangerLevel.medium, _levelContentContainer.LevelParameters.MediumDangerPropability},
            { DangerLevel.high, _levelContentContainer.LevelParameters.HighDangerPropability}
        };
        propabilities.OrderBy(a => a.Value);
        float d100 = UnityEngine.Random.Range(0, 100);
        foreach (KeyValuePair<DangerLevel, float> prop in propabilities)
        {
            if (prop.Value <= d100)
            {
                outcome = prop.Key;
                break;
            }
        }

        OutcomeData outcomeData = new OutcomeData()
        {
            Outcome = outcome,
            Decision = _levelContentContainer.Decisions.FirstOrDefault(a => a.DangerLevel == dangerLevel),
            Level = _levelContentContainer.LevelParameters
        };
        _outcomes.Add(outcomeData);

        OutcomeCalculated.Invoke(outcomeData);
    }
}
