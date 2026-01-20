using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OutcomeUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    [SerializeField] private VisualTreeAsset outcomeOverviewAsset;
    [SerializeField] private VisualTreeAsset outcomeEntryAsset;
    [SerializeField] private VisualTreeAsset outcomeSubEntryAsset;

    private VisualElement _backGround;
    private Button _buttonRequestEvaluation;

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
        // Assemble Outcome Overview
        VisualElement overviewContainer = outcomeOverviewAsset.Instantiate().Q<VisualElement>("OverviewContainer");
        _backGround.Add(overviewContainer);
        _buttonRequestEvaluation = overviewContainer.Q<Button>("Button_RequestEvaluation");
        _buttonRequestEvaluation.clicked += TriggerEvaluation;

        foreach (OutcomeData outcome in outcomes)
        {
            VisualElement outcomeEntry = outcomeEntryAsset.Instantiate().Q<VisualElement>("OutcomeEntry");
            overviewContainer.Add(outcomeEntry);

            Label outcomeDescription = outcomeEntry.Q<Label>("OutcomeDescription");
            switch (outcome.Outcome)
            {
                case DangerLevel.high:
                    outcomeDescription.text = outcome.Level.HighOutcomeDescription;
                    break;

                case DangerLevel.medium:
                    outcomeDescription.text = outcome.Level.MediumOutcomeDescription;
                    break;

                case DangerLevel.low:
                    outcomeDescription.text = outcome.Level.LowOutcomeDescription;
                    break;
            }

            VisualElement subEntryList = outcomeEntry.Q<VisualElement>("SubEntryList");
            foreach (DecisionDataEntry decisionEntry in outcome.Decision.Contents)
            {
                VisualElement outcomeSubEntry = outcomeSubEntryAsset.Instantiate().Q<VisualElement>("OutcomeSubEntry");
                subEntryList.Add(outcomeSubEntry);

                Label decision = outcomeSubEntry.Q<Label>("Decision");
                decision.text = decisionEntry.ActionDescription;

                Label decicionOutcome = outcomeSubEntry.Q<Label>("DecisionOutcome");
                switch (outcome.Outcome)
                {
                    case DangerLevel.high:
                        decicionOutcome.text = decisionEntry.HighDangerOutcome;
                        break;

                    case DangerLevel.medium:
                        decicionOutcome.text = decisionEntry.MediumDangerOutcome;
                        break;

                    case DangerLevel.low:
                        decicionOutcome.text = decisionEntry.LowDangerOutcome;
                        break;
                }
            }
        }
    }

    private void TriggerEvaluation()
    {
        //_buttonRequestEvaluation.clicked -= TriggerEvaluation;
        Debug.Log("Show Evaluation");
    }
}
