using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    [SerializeField] private VisualTreeAsset modalContainerAsset;
    [SerializeField] private VisualTreeAsset previewEntryAsset;
    [SerializeField] private VisualTreeAsset decisionEntryAsset;

    public static event Action<DangerLevel> DecisionMade;

    private VisualElement _backGround;
    private VisualElement _weatherMap;
    private VisualElement _modalContainer;
    private Button _buttonClose;
    private Button _buttonDecision;

    private List<Button> _indicatorButtons;
    private List<ConsequencePreview> _consequences = new List<ConsequencePreview>();

    private List<Button> _decisionButtons;
    private List<DecisionData> _decisions = new List<DecisionData>();

    void OnEnable()
    {
        GameManager.RoundStarted += OnNewDataIncoming;
        _backGround = ui.rootVisualElement.Q<VisualElement>("BackGround");
    }

    private void OnDisable()
    {        
        GameManager.RoundStarted -= OnNewDataIncoming;
    }

    private void OnNewDataIncoming(List<ConsequencePreview> consequences, List<DecisionData> decisions, VisualTreeAsset mapAsset)
    {
        if (_weatherMap != null)
        {
            _backGround.Clear();
            _indicatorButtons[0].clicked -= OnHighDangerButtonClicked;
            _indicatorButtons[1].clicked -= OnMediumDangerButtonClicked;
            _indicatorButtons[2].clicked -= OnLowDangerButtonClicked;
            _buttonDecision.clicked -= OnShowDecisionsClick;
        }

        _consequences = consequences;
        _decisions = decisions.OrderByDescending(a => a.DangerLevel).ToList();
        _weatherMap = mapAsset.Instantiate().Q<VisualElement>("WeatherMap");
        _backGround.Add(_weatherMap);

        _indicatorButtons = _weatherMap.Query<Button>("Indicator_Nob").ToList();
        _indicatorButtons[0].clicked += OnHighDangerButtonClicked;
        _indicatorButtons[1].clicked += OnMediumDangerButtonClicked;
        _indicatorButtons[2].clicked += OnLowDangerButtonClicked;

        _buttonDecision = _weatherMap.Q<Button>("Button_Decision");
        _buttonDecision.clicked += OnShowDecisionsClick;
    }

    private void OnHighDangerButtonClicked()
    {
        OnIndicatorClick(DangerLevel.high);
    }
    private void OnMediumDangerButtonClicked()
    {
        OnIndicatorClick(DangerLevel.medium);
    }
    private void OnLowDangerButtonClicked()
    {
        OnIndicatorClick(DangerLevel.low);
    }

    private void OnIndicatorClick(DangerLevel dangerLevel)
    {
        ConsequencePreview currentPreview = _consequences.FirstOrDefault(a => a.DangerLevel == dangerLevel);

        _modalContainer = modalContainerAsset.Instantiate().Q<VisualElement>("ModalContainer");
        _backGround.Add(_modalContainer);

        _buttonClose = _modalContainer.Q<Button>("Button_Cancel");
        _buttonClose.clicked += OnClosePreviewClick;

        VisualElement previewList = _modalContainer.Q<VisualElement>("EntryList");
        foreach (ConsequencePreviewEntry entry in currentPreview.entries)
        {
            VisualElement previewUIElement = previewEntryAsset.Instantiate().Q<VisualElement>("Entry");

            VisualElement profilePic = previewUIElement.Q<VisualElement>("ProfilePic");
            Label text = previewUIElement.Q<Label>("Text");

            profilePic.style.backgroundImage = new StyleBackground(entry.ProfileImage);
            text.text = entry.Text;
            previewList.Add(previewUIElement);
        }
    }

    private void OnClosePreviewClick()
    {
        _buttonClose.clicked -= OnClosePreviewClick;
        _backGround.Remove(_modalContainer);
    }

    private void OnShowDecisionsClick()
    {
        _modalContainer = modalContainerAsset.Instantiate().Q<VisualElement>("ModalContainer");
        _backGround.Add(_modalContainer);

        _buttonClose = _modalContainer.Q<Button>("Button_Cancel");
        _buttonClose.clicked += OnCloseDecisionsClick;

        VisualElement decisionList = _modalContainer.Q<VisualElement>("EntryList");
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
