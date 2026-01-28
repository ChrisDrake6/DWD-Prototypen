using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TransitionUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    [SerializeField] private VisualTreeAsset transitionAsset;

    public static event Action TransitionClosed;

    private VisualElement _backGround;
    private VisualElement _transitionContainer;
    private Button _buttonContinue;
    private Label _text;

    private void OnEnable()
    {
        GameManager.OutcomeCalculated += OnOutcomeCalculated;
    }

    private void OnDisable()
    {
        GameManager.OutcomeCalculated -= OnOutcomeCalculated;
    }

    private void OnOutcomeCalculated(OutcomeData outcome)
    {
        _backGround = ui.rootVisualElement.Q<VisualElement>("BackGround");
        _transitionContainer = transitionAsset.Instantiate().Q<VisualElement>("TransitionContainer");
        _backGround.Add(_transitionContainer);

        _buttonContinue = _transitionContainer.Q<Button>("Button_Continue");
        _buttonContinue.clicked += OnContinueClick;

        _text = _transitionContainer.Q<Label>("Text");

        // Nifty: https://stackoverflow.com/questions/44355630/how-to-use-c-sharp-tuple-value-types-in-a-switch-statement
        var dangerLevels = new Tuple<DangerLevel, DangerLevel>(outcome.Outcome, outcome.Decision.DangerLevel);
        _text.text = dangerLevels switch
        {
            (DangerLevel.low, DangerLevel.low) => outcome.Level.LowOutComeLowDecisionTransitionText,
            (DangerLevel.low, DangerLevel.medium) => outcome.Level.LowOutComeMediumDecisionTransitionText,
            (DangerLevel.low, DangerLevel.high) => outcome.Level.LowOutComeHighDecisionTransitionText,
            (DangerLevel.medium, DangerLevel.low) => outcome.Level.MediumOutComeLowDecisionTransitionText,
            (DangerLevel.medium, DangerLevel.medium) => outcome.Level.MediumOutComeMediumDecisionTransitionText,
            (DangerLevel.medium, DangerLevel.high) => outcome.Level.MediumOutComeHighDecisionTransitionText,
            (DangerLevel.high, DangerLevel.low) => outcome.Level.HighOutComeLowDecisionTransitionText,
            (DangerLevel.high, DangerLevel.medium) => outcome.Level.HighOutComeMediumDecisionTransitionText,
            (DangerLevel.high, DangerLevel.high) => outcome.Level.HighOutComeHighDecisionTransitionText,
            _ => "Something went horribly wrong."
        };
    }

    private void OnContinueClick()
    {
        _buttonContinue.clicked -= OnContinueClick;
        _backGround.Remove(_transitionContainer);
        TransitionClosed.Invoke();
    }
}
