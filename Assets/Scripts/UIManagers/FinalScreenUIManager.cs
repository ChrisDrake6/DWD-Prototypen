using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class FinalScreenUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    [SerializeField] private VisualTreeAsset finalScreenAsset;

    [SerializeField] private Sprite lowOutcomeLowDecisionBackGround;
    [SerializeField] private Sprite lowOutcomeMediumDecisionBackGround;
    [SerializeField] private Sprite lowOutcomeHighDecisionBackGround;
    [SerializeField] private Sprite mediumOutcomeLowDecisionBackGround;
    [SerializeField] private Sprite mediumOutcomeMediumDecisionBackGround;
    [SerializeField] private Sprite mediumOutcomeHighDecisionBackGround;
    [SerializeField] private Sprite highOutcomeLowDecisionBackGround;
    [SerializeField] private Sprite highOutcomeMediumDecisionBackGround;
    [SerializeField] private Sprite highOutcomeHighDecisionBackGround;

    private Button _buttonRestart;
    private Button _buttonQuit;

    private void OnEnable()
    {
        OutcomeUIManager.EvaluationTriggered += SetBackGround;
    }

    private void OnDisable()
    {
        OutcomeUIManager.EvaluationTriggered -= SetBackGround;
    }

    private void SetBackGround(List<OutcomeData> outcomes)
    {
        VisualElement backGround = ui.rootVisualElement.Q<VisualElement>("BackGround");
        VisualElement finalScreenContainer = finalScreenAsset.Instantiate().Q<VisualElement>("FinalScreenContainer");
        backGround.Add(finalScreenContainer);
        _buttonRestart = finalScreenContainer.Q<Button>("Button_Restart");
        _buttonQuit = finalScreenContainer.Q<Button>("Button_Quit");
        _buttonRestart.clicked += RestartGame;
        _buttonQuit.clicked += QuitGame;

        // TODO: Replace this with a more variable version, possibly using Cost / Lost
        var dangerLevels = new Tuple<DangerLevel, DangerLevel>(outcomes[0].Outcome, outcomes[1].Outcome);
        DangerLevel generalOutcome = dangerLevels switch
        {
            (DangerLevel.low, DangerLevel.low) => DangerLevel.low,
            (DangerLevel.low, DangerLevel.medium) => DangerLevel.medium,
            (DangerLevel.low, DangerLevel.high) => DangerLevel.high,
            (DangerLevel.medium, DangerLevel.low) => DangerLevel.low,
            (DangerLevel.medium, DangerLevel.medium) => DangerLevel.medium,
            (DangerLevel.medium, DangerLevel.high) => DangerLevel.high,
            (DangerLevel.high, DangerLevel.low) => DangerLevel.medium,
            (DangerLevel.high, DangerLevel.medium) => DangerLevel.medium,
            (DangerLevel.high, DangerLevel.high) => DangerLevel.high,
            _ => DangerLevel.high
        };

        dangerLevels = new Tuple<DangerLevel, DangerLevel>(outcomes[0].Decision.DangerLevel, outcomes[1].Decision.DangerLevel);
        DangerLevel generalDecisionLevel = dangerLevels switch
        {
            (DangerLevel.low, DangerLevel.low) => DangerLevel.low,
            (DangerLevel.low, DangerLevel.medium) => DangerLevel.medium,
            (DangerLevel.low, DangerLevel.high) => DangerLevel.high,
            (DangerLevel.medium, DangerLevel.low) => DangerLevel.low,
            (DangerLevel.medium, DangerLevel.medium) => DangerLevel.medium,
            (DangerLevel.medium, DangerLevel.high) => DangerLevel.high,
            (DangerLevel.high, DangerLevel.low) => DangerLevel.medium,
            (DangerLevel.high, DangerLevel.medium) => DangerLevel.medium,
            (DangerLevel.high, DangerLevel.high) => DangerLevel.high,
            _ => DangerLevel.high
        };

        dangerLevels = new Tuple<DangerLevel, DangerLevel>(outcomes[0].Decision.DangerLevel, outcomes[1].Decision.DangerLevel);
        backGround.style.backgroundImage = dangerLevels switch
        {
            (DangerLevel.low, DangerLevel.low) => new StyleBackground(lowOutcomeLowDecisionBackGround),
            (DangerLevel.low, DangerLevel.medium) => new StyleBackground(lowOutcomeMediumDecisionBackGround),
            (DangerLevel.low, DangerLevel.high) => new StyleBackground(lowOutcomeHighDecisionBackGround),
            (DangerLevel.medium, DangerLevel.low) => new StyleBackground(mediumOutcomeLowDecisionBackGround),
            (DangerLevel.medium, DangerLevel.medium) => new StyleBackground(mediumOutcomeMediumDecisionBackGround),
            (DangerLevel.medium, DangerLevel.high) => new StyleBackground(mediumOutcomeHighDecisionBackGround),
            (DangerLevel.high, DangerLevel.low) => new StyleBackground(highOutcomeLowDecisionBackGround),
            (DangerLevel.high, DangerLevel.medium) => new StyleBackground(highOutcomeMediumDecisionBackGround),
            (DangerLevel.high, DangerLevel.high) => new StyleBackground(highOutcomeHighDecisionBackGround),
            _ => null
        };
    }

    private void RestartGame()
    {
        _buttonRestart.clicked -= RestartGame;
        _buttonQuit.clicked -= QuitGame;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitGame() 
    {
        _buttonRestart.clicked -= RestartGame;
        _buttonQuit.clicked -= QuitGame;
        SceneManager.LoadScene(0);
    }
}
