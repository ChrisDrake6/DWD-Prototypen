using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DecisionsUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    [SerializeField] private VisualTreeAsset modalContainerAsset;
    [SerializeField] private VisualTreeAsset decisionEntryAsset;

    public static event Action<DangerLevel> DecisionMade;
    public static event Action WindowClosed;

    private VisualElement _backGround;
    private VisualElement _modalContainer;
    private Button _buttonClose;
    private List<DecisionData> _decisions = new List<DecisionData>();
    private List<Button> _decisionButtons;

    private void OnEnable()
    {
        _backGround = ui.rootVisualElement.Q<VisualElement>("BackGround");
        GameManager.RoundStarted += OnNewDataIncoming;
        WeatherMapUIManager.DecisionButtonClicked += OnShowDecisionsClick;
    }

    private void OnDisable()
    {
        GameManager.RoundStarted -= OnNewDataIncoming;
        WeatherMapUIManager.DecisionButtonClicked -= OnShowDecisionsClick;
    }

    private void OnNewDataIncoming(LevelContentContainer levelData)
    {
        _decisions = levelData.Decisions.OrderByDescending(a => a.DangerLevel).ToList();
    }

    private void OnShowDecisionsClick()
    {
        _modalContainer = modalContainerAsset.Instantiate().Q<VisualElement>("ModalContainer");
        _backGround.Add(_modalContainer);

        _buttonClose = _modalContainer.Q<Button>("Button_Cancel");
        _buttonClose.clicked += OnCloseDecisionsClick;

        ScrollView decisionList = _modalContainer.Q<ScrollView>("EntryList");
        foreach (DecisionData decision in _decisions)
        {
            VisualElement decisionUIElement = decisionEntryAsset.Instantiate().Q<VisualElement>("Entry");

            Label dangerLevel = decisionUIElement.Q<Label>("DangerLevel");
            switch (decision.DangerLevel)
            {
                case DangerLevel.high:
                    dangerLevel.text = "Extreme Maﬂnahmen:";
                    break;
                case DangerLevel.medium:
                    dangerLevel.text = "Versch‰rfte Maﬂnahmen:";

                    break;
                case DangerLevel.low:
                    dangerLevel.text = "Leichte Maﬂnahmen:";
                    break;
            }

            Label text = decisionUIElement.Q<Label>("Text");
            foreach (DecisionDataEntry entry in decision.Contents)
            {
                text.text += $"- {entry.ActionDescription}\n";
            }
            decisionList.Add(decisionUIElement);
        }

        _decisionButtons = decisionList.Query<Button>("Button_Choose").ToList();
        _decisionButtons[0].clicked += OnHighAlertDecisionClick;
        _decisionButtons[1].clicked += OnMediumAlertDecisionClick;
        _decisionButtons[2].clicked += OnLowAlertDecisionClick;
    }

    private void OnCloseDecisionsClick()
    {
        _buttonClose.clicked -= OnCloseDecisionsClick;
        _decisionButtons[0].clicked -= OnHighAlertDecisionClick;
        _decisionButtons[1].clicked -= OnMediumAlertDecisionClick;
        _decisionButtons[2].clicked -= OnLowAlertDecisionClick;
        _backGround.Remove(_modalContainer);
        WindowClosed.Invoke();
    }

    private void OnHighAlertDecisionClick()
    {
        OnCloseDecisionsClick();
        DecisionMade.Invoke(DangerLevel.high);
    }
    private void OnMediumAlertDecisionClick()
    {
        OnCloseDecisionsClick();
        DecisionMade.Invoke(DangerLevel.medium);
    }
    private void OnLowAlertDecisionClick()
    {
        OnCloseDecisionsClick();
        DecisionMade.Invoke(DangerLevel.low);
    }
}
