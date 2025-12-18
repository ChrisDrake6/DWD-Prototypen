using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static event Action<List<ConsequencePreview>, List<DecisionData>, VisualTreeAsset> RoundStarted;
    public static event Action<List<OutcomeData>> LastDecisionMade;

    private string _path;
    private string[] directories;
    private int _currentRound;

    private List<ConsequencePreview> _consequences = new List<ConsequencePreview>();
    private List<DecisionData> _decisions = new List<DecisionData>();
    private LevelData _levelData;

    private List<OutcomeData> _outcomes = new List<OutcomeData>();

    void Start()
    {
        _path = Path.Combine("Assets", "Levels");
        directories = Directory.GetDirectories(_path);
        UIManager.DecisionMade += OnDecisionMade;
        StartNewRound();
    }

    private void OnDestroy()
    {
        UIManager.DecisionMade -= OnDecisionMade;        
    }

    private void StartNewRound()
    {
        if (_currentRound < directories.Length)
        {
            // Get Files and distribute Data.
            _levelData = CustomUtils.GetSOAssets<LevelData>(directories[_currentRound])[0];
            _consequences = CustomUtils.GetSOAssets<ConsequencePreview>(Path.Combine(directories[_currentRound], "Consequences"));
            _decisions = CustomUtils.GetSOAssets<DecisionData>(Path.Combine(directories[_currentRound], "Decisions"));

            RoundStarted.Invoke(_consequences, _decisions, _levelData.MapAsset);

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
            { DangerLevel.medium, _levelData.MediumDangerPropability},
            { DangerLevel.high, _levelData.HighDangerPropability}
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

        _outcomes.Add(new OutcomeData()
        {
            Outcome = outcome,
            Decision = _decisions.FirstOrDefault(a => a.DangerLevel == dangerLevel),
            Level = _levelData
        });

        StartNewRound();
    }
}
