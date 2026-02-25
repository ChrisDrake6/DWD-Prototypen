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
                    decisionUIElement.RegisterCallbackOnce<ClickEvent>(OnHighAlertDecisionClick);
                    break;
                case DangerLevel.medium:
                    dangerLevel.text = "Versch‰rfte Maﬂnahmen:";
                    decisionUIElement.RegisterCallbackOnce<ClickEvent>(OnMediumAlertDecisionClick);
                    break;
                case DangerLevel.low:
                    dangerLevel.text = "Leichte Maﬂnahmen:";
                    decisionUIElement.RegisterCallbackOnce<ClickEvent>(OnLowAlertDecisionClick);
                    break;
            }

            Label text = decisionUIElement.Q<Label>("Text");
            text.text = decision.ActionDescription;
            decisionList.Add(decisionUIElement);
        }
    }

    private void OnCloseDecisionsClick()
    {
        _buttonClose.clicked -= OnCloseDecisionsClick;
        _backGround.Remove(_modalContainer);
        WindowClosed.Invoke();
    }

    private void OnHighAlertDecisionClick(ClickEvent ev)
    {
        OnCloseDecisionsClick();
        DecisionMade.Invoke(DangerLevel.high);
    }
    private void OnMediumAlertDecisionClick(ClickEvent ev)
    {
        OnCloseDecisionsClick();
        DecisionMade.Invoke(DangerLevel.medium);
    }
    private void OnLowAlertDecisionClick(ClickEvent ev)
    {
        OnCloseDecisionsClick();
        DecisionMade.Invoke(DangerLevel.low);
    }
}
