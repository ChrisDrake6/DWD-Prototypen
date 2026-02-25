using System.Collections.Generic;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class HelperUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    [SerializeField] private HelperData helperData;

    public static event Action OnBoardingDone;

    private VisualElement _helperContainer;
    private VisualElement _blocker;
    private VisualElement _helperProfile;
    private VisualElement _indicator;
    private VisualElement _speechBubble;
    private VisualElement _image;
    private Label _text;

    private HelperDataEntry[] _currectHelperDataEntries;
    private int _index = 0;
    private bool _isOnBoarding = true;
    private int _alreadySeenMask = 0;
    private int _currentPositionInMask = 0;

    private void OnEnable()
    {
        _helperContainer = ui.rootVisualElement.Q<VisualElement>("HelperContainer");
        _blocker = _helperContainer.Q<VisualElement>("Blocker");
        _helperProfile = _helperContainer.Q<VisualElement>("HelperProfile");
        _indicator = _helperContainer.Q<VisualElement>("Indicator");
        _speechBubble = _helperContainer.Q<VisualElement>("SpeechBubble");
        _image = _speechBubble.Q<VisualElement>("Image");
        _text = _speechBubble.Q<Label>("Text");

        _helperProfile.RegisterCallback<ClickEvent>(OnHelperClick);
        _speechBubble.RegisterCallback<ClickEvent>(OnBubbleClick);

        _currectHelperDataEntries = helperData.OnBoardingLines;
        SetBubbleContent();

        GameManager.RoundStarted += OnNewRoundStarted;
        GameManager.OutcomeCalculated += SetTransitionLines;
        GameManager.LastDecisionMade += SetOutcomeLines;
        WeatherMapUIManager.IndicatorClicked += SetDangerPreviewLines;
        WeatherMapUIManager.DecisionButtonClicked += SetDecisionLines;
        DangerPreviewUIManager.WindowClosed += SetWeatherMapLines;
        DecisionsUIManager.WindowClosed += SetWeatherMapLines;
        OutcomeUIManager.EvaluationTriggered += SetFinalScreenLines;
    }

    private void OnDisable()
    {
        _helperProfile.UnregisterCallback<ClickEvent>(OnHelperClick);
        _speechBubble.UnregisterCallback<ClickEvent>(OnBubbleClick);

        GameManager.RoundStarted -= OnNewRoundStarted;
        GameManager.OutcomeCalculated -= SetTransitionLines;
        GameManager.LastDecisionMade -= SetOutcomeLines;
        WeatherMapUIManager.IndicatorClicked -= SetDangerPreviewLines;
        WeatherMapUIManager.DecisionButtonClicked -= SetDecisionLines;
        DangerPreviewUIManager.WindowClosed -= SetWeatherMapLines;
        DecisionsUIManager.WindowClosed -= SetWeatherMapLines;
        OutcomeUIManager.EvaluationTriggered -= SetFinalScreenLines;
    }

    private void OnHelperClick(ClickEvent ev)
    {
        if (!_speechBubble.visible)
        {
            _speechBubble.visible = true;
            _blocker.visible = true;
            SetBubbleContent();
        }
        else
        {
            HideBubble();
        }
    }

    private void OnBubbleClick(ClickEvent ev)
    {
        if (_index < _currectHelperDataEntries.Length)
        {
            SetBubbleContent();
        }
        else
        {
            HideBubble();
        }
    }

    private void SetBubbleContent()
    {
        _text.text = _currectHelperDataEntries[_index].Text;
        _indicator.AddToClassList("helperIndicatorInactive");
        if (!_isOnBoarding)
        {
            _alreadySeenMask |= 1 << _currentPositionInMask;
        }
        if (_currectHelperDataEntries[_index].Image != null)
        {
            _image.style.display = DisplayStyle.Flex;
            _image.style.backgroundImage = new StyleBackground(_currectHelperDataEntries[_index].Image);
            _text.AddToClassList("text_flowing_small");
        }
        else
        {
            _image.style.display = DisplayStyle.None;
            _text.RemoveFromClassList("text_flowing_small");
        }
        _index++;
    }

    private void HideBubble()
    {
        _speechBubble.visible = false;
        _blocker.visible = false;
        _index = 0;
        if (_isOnBoarding)
        {
            OnBoardingDone.Invoke();
            _isOnBoarding = false;
        }
    }

    private void OnNewRoundStarted(LevelContentContainer levelData)
    {
        SetWeatherMapLines();
    }

    private void SetWeatherMapLines()
    {
        _currentPositionInMask = 0;
        CheckForNewLines();
        _currectHelperDataEntries = helperData.WeatherMapLines;
    }

    private void SetDangerPreviewLines(DangerLevel dangerLevel)
    {
        _currentPositionInMask = 1;
        CheckForNewLines();
        _currectHelperDataEntries = helperData.DangerPreviewLines;
    }

    private void SetDecisionLines()
    {
        _currentPositionInMask = 2;
        CheckForNewLines();
        _currectHelperDataEntries = helperData.DecisionLines;
    }

    private void SetTransitionLines(OutcomeData outcomeData)
    {
        _currentPositionInMask = 3;
        CheckForNewLines();
        _currectHelperDataEntries = helperData.TransitionLines;
    }

    private void SetOutcomeLines(List<OutcomeData> outcomes)
    {
        _currentPositionInMask = 4;
        CheckForNewLines();
        _currectHelperDataEntries = helperData.OutcomeLines;
    }

    private void SetFinalScreenLines(List<OutcomeData> outcomes)
    {
        _currentPositionInMask = 5;
        CheckForNewLines();
        _currectHelperDataEntries = helperData.FinalScreenLines;
    }

    private void CheckForNewLines()
    {
        if ((_alreadySeenMask & 1 << _currentPositionInMask) == 0)
        {
            _indicator.RemoveFromClassList("helperIndicatorInactive");
        }
        else
        {
            _indicator.AddToClassList("helperIndicatorInactive");
        }
    }
}
