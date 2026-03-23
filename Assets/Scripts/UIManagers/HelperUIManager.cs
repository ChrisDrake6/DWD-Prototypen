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

    /// <summary>
    /// Build Helper UI
    /// </summary>
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
        _indicator.RegisterCallback<TransitionEndEvent>(AnimateIndicator);

        // Start with Onboarding
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
        _indicator.UnregisterCallback<TransitionEndEvent>(AnimateIndicator);

        GameManager.RoundStarted -= OnNewRoundStarted;
        GameManager.OutcomeCalculated -= SetTransitionLines;
        GameManager.LastDecisionMade -= SetOutcomeLines;
        WeatherMapUIManager.IndicatorClicked -= SetDangerPreviewLines;
        WeatherMapUIManager.DecisionButtonClicked -= SetDecisionLines;
        DangerPreviewUIManager.WindowClosed -= SetWeatherMapLines;
        DecisionsUIManager.WindowClosed -= SetWeatherMapLines;
        OutcomeUIManager.EvaluationTriggered -= SetFinalScreenLines;
    }

    /// <summary>
    /// Either hides or shows bubble - this will cancel any chain of entries.
    /// </summary>
    /// <param name="ev"></param>
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

    /// <summary>
    /// Show next line entry or hide bubble if none are left.
    /// </summary>
    /// <param name="ev"></param>
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

    /// <summary>
    /// Manages Text and Indicator
    /// </summary>
    private void SetBubbleContent()
    {
        _text.text = _currectHelperDataEntries[_index].Text;
        _indicator.AddToClassList("helperIndicatorInactive");

        // Once OnBoarding is over, we can use the bit mask.
        if (!_isOnBoarding)
        {
            // Set the bit of the current entry to 1. (for example, if the current position is 2, set the 3rd bit from the end to 1. (0 is also a position.))
            _alreadySeenMask |= 1 << _currentPositionInMask;
        }

        // If there is an image, show it.
        if (_currectHelperDataEntries[_index].Image != null)
        {
            _image.style.display = DisplayStyle.Flex;
            _image.style.minHeight = _currectHelperDataEntries[_index].Image.textureRect.height;
            _image.style.backgroundImage = new StyleBackground(_currectHelperDataEntries[_index].Image);
        }
        else
        {
            _image.style.display = DisplayStyle.None;
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
            // Gives the gamemanager an event, so the game can start.
            OnBoardingDone.Invoke();
            _isOnBoarding = false;
        }
    }

    /// <summary>
    /// Gets triggered, if Gamemanager starts new round.
    /// </summary>
    /// <param name="levelData"></param>
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

    /// <summary>
    /// This checks, if the helpertext, which is ready to be shown, has already been read or not. The indicator should only show up, if there is new content to read.
    /// In order to do that, we use a bitmask. Each entry gets assigned to a bit in the mask. If it has never been shown, its bit is set to 0 by default (false). Once it has been shown, we manually set it to 1 (true).
    /// </summary>
    private void CheckForNewLines()
    {
        // check if the corresponding bit of the current entry is set to 0.
        if ((_alreadySeenMask & 1 << _currentPositionInMask) == 0)
        {
            _indicator.RemoveFromClassList("helperIndicatorInactive");
        }
        else
        {
            _indicator.AddToClassList("helperIndicatorInactive");
        }
    }

    /// <summary>
    /// This provides an animation loop for the indicator.
    /// </summary>
    /// <param name="endEvent"></param>
    private void AnimateIndicator(TransitionEndEvent endEvent)
    {
        _indicator.ToggleInClassList("helperIndicatorUp");
    }
}
