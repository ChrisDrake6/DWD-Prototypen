using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OutcomeUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    [SerializeField] private VisualTreeAsset outcomeOverviewAsset;
    [SerializeField] private VisualTreeAsset outcomeEntryAsset;

    public static event Action<List<OutcomeData>> EvaluationTriggered;

    private VisualElement _backGround;
    VisualElement _overviewContainer;
    private Button _buttonRequestEvaluation;
    private List<OutcomeData> _outcomes;

    private void OnEnable()
    {
        _backGround = ui.rootVisualElement.Q<VisualElement>("BackGround");
        GameManager.LastDecisionMade += ShowOutcomes;
    }

    private void OnDisable()
    {
        GameManager.LastDecisionMade -= ShowOutcomes;
    }

    private void ShowOutcomes(List<OutcomeData> outcomes)
    {
        _outcomes = outcomes;
        // Assemble Outcome Overview
        _overviewContainer = outcomeOverviewAsset.Instantiate().Q<VisualElement>("OverviewContainer");
        _backGround.Add(_overviewContainer);
        _buttonRequestEvaluation = _overviewContainer.Q<Button>("Button_RequestEvaluation");
        _buttonRequestEvaluation.clicked += TriggerEvaluation;

        foreach (OutcomeData outcome in outcomes)
        {
            VisualElement outcomeEntry = outcomeEntryAsset.Instantiate().Q<VisualElement>("OutcomeEntry");
            ScrollView entryList = _overviewContainer.Q<ScrollView>("EntryList");

            entryList.Add(outcomeEntry);

            Label outcomeDescription = outcomeEntry.Q<Label>("OutcomeDescription");
            switch (outcome.Outcome)
            {
                case DangerLevel.high:
                    outcomeDescription.text = outcome.Level.TimeDisplayed + ": " + outcome.Level.HighOutcomeDescription;
                    break;

                case DangerLevel.medium:
                    outcomeDescription.text = outcome.Level.TimeDisplayed + ": " + outcome.Level.MediumOutcomeDescription;
                    break;

                case DangerLevel.low:
                    outcomeDescription.text = outcome.Level.TimeDisplayed + ": " + outcome.Level.LowOutcomeDescription;
                    break;
            }

            Label decision = outcomeEntry.Q<Label>("Decision");
            decision.text = outcome.Decision.ActionDescription;


            Label extraText = outcomeEntry.Q<Label>("ExtraText");
            VisualElement outcomeSubEntry = outcomeEntry.Q<VisualElement>("OutcomeSubEntry");
            Label decicionOutcome = outcomeEntry.Q<Label>("DecisionOutcome");
            switch (outcome.Outcome)
            {
                case DangerLevel.high:
                    CheckForMissingText(outcome.Decision.HighDangerOutcome, outcomeEntry);
                    decicionOutcome.text = outcome.Decision.HighDangerOutcome;
                    break;

                case DangerLevel.medium:
                    CheckForMissingText(outcome.Decision.MediumDangerOutcome, outcomeEntry);
                    decicionOutcome.text = outcome.Decision.MediumDangerOutcome;
                    break;

                case DangerLevel.low:
                    CheckForMissingText(outcome.Decision.LowDangerOutcome, outcomeEntry);
                    decicionOutcome.text = outcome.Decision.LowDangerOutcome;
                    break;
            }
        }
    }

    private void CheckForMissingText(string text, VisualElement outcomeEntry)
    {
        if(string.IsNullOrEmpty(text))
        {
            Label extraText = outcomeEntry.Q<Label>("ExtraText");
            VisualElement outcomeSubEntry = outcomeEntry.Q<VisualElement>("OutcomeSubEntry");
            extraText.style.display = DisplayStyle.None;
            outcomeSubEntry.style.display = DisplayStyle.None;
        }
    }

    private void TriggerEvaluation()
    {
        _buttonRequestEvaluation.clicked -= TriggerEvaluation;
        _backGround.Remove(_overviewContainer);
        EvaluationTriggered.Invoke(_outcomes);
    }
}
