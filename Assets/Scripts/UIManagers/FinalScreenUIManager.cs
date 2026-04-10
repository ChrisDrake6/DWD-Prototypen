using System;
using System.Collections.Generic;
using System.Linq;
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
    private Button _buttonEvaluate;
    private Button _buttonQuit;

    private void OnEnable()
    {
        OutcomeUIManager.EvaluationTriggered += SetBackGround;
    }

    private void OnDisable()
    {
        OutcomeUIManager.EvaluationTriggered -= SetBackGround;
    }

    /// <summary>
    /// Gets triggered on continuing from the outcome page.
    /// Pick the correct image according to the weather incidents and the decisions made.
    /// </summary>
    /// <param name="outcomes"></param>
    private void SetBackGround(List<OutcomeData> outcomes)
    {
        VisualElement backGround = ui.rootVisualElement.Q<VisualElement>("BackGround");
        VisualElement finalScreenContainer = finalScreenAsset.Instantiate().Q<VisualElement>("FinalScreenContainer");
        backGround.Add(finalScreenContainer);
        _buttonRestart = finalScreenContainer.Q<Button>("Button_Restart");
        _buttonEvaluate = finalScreenContainer.Q<Button>("Button_Evaluate");
        _buttonQuit = finalScreenContainer.Q<Button>("Button_Quit");
        _buttonRestart.clicked += RestartGame;
        _buttonEvaluate.clicked += OpenEvaluation;
        _buttonQuit.clicked += QuitGame;

        // In order to avoid having to use 9^x images and to use only 9, we need a way to determine the average result of either the outcome of the weather and the decisions.
        // For that, we use the fact, that any enum is stored as an integer by default. We convert the Dangerlevels to int, take the average, round it,
        // so that the result is either 1, 2 or 3 and convert it back into the enum. 
        DangerLevel generalOutcome = (DangerLevel)Math.Round(outcomes.Average(a => (int)a.Outcome));
        DangerLevel generalDecisionLevel = (DangerLevel)Math.Round(outcomes.Average(a => (int)a.Decision.DangerLevel));

        // Create a 'matrix' of outcomes and decisions, 3 rows by 3 collums, resulting in 9 entries. Each entry of the matrix is one image.
        var dangerLevels = new Tuple<DangerLevel, DangerLevel>(generalOutcome, generalDecisionLevel);
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

    /// <summary>
    /// Restarts level
    /// </summary>
    private void RestartGame()
    {
        _buttonRestart.clicked -= RestartGame;
        _buttonEvaluate.clicked -= OpenEvaluation;
        _buttonQuit.clicked -= QuitGame;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Opens a online survey for testing
    /// TODO: REMOVE THIS, ONCE TESTING IS DONE!!!
    /// </summary>
    private void OpenEvaluation()
    {
        Application.OpenURL("https://www.empirio.de/s/0W2ibtW1FV");
    }

    /// <summary>
    /// Will return to main menu, since this is a web build.
    /// </summary>
    private void QuitGame() 
    {
        _buttonRestart.clicked -= RestartGame;
        _buttonQuit.clicked -= QuitGame;
        SceneManager.LoadScene(0);
    }
}
