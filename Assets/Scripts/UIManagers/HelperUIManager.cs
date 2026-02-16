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
    private VisualElement _helperProfile;
    private VisualElement _speechBubble;
    private VisualElement _image;
    private Label _text;

    private HelperDataEntry[] _currectHelperDataEntries;
    private int _index = 0;
    private bool _isOnBoarding = true;

    private void OnEnable()
    {
        _helperContainer = ui.rootVisualElement.Q<VisualElement>("HelperContainer");
        _helperProfile = _helperContainer.Q<VisualElement>("HelperProfile");
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
        OutcomeUIManager.EvaluationTriggered += DisableHelper;
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
        OutcomeUIManager.EvaluationTriggered -= DisableHelper;
    }

    private void OnHelperClick(ClickEvent ev)
    {
        if (!_speechBubble.visible)
        {
            _speechBubble.visible = true;
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
        _index = 0;
        if(_isOnBoarding)
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
        _currectHelperDataEntries = helperData.WeatherMapLines;
    }

    private void SetTransitionLines(OutcomeData outcomeData) 
    {
        _currectHelperDataEntries = helperData.TransitionLines;
    }

    private void SetDangerPreviewLines(DangerLevel dangerLevel)
    {
        _currectHelperDataEntries = helperData.DangerPreviewLines;
    }

    private void SetDecisionLines()
    {
        _currectHelperDataEntries = helperData.DecisionLines;
    }

    private void SetOutcomeLines(List<OutcomeData> outcomes)
    {
        _currectHelperDataEntries = helperData.OutcomeLines;
    }

    private void DisableHelper(List<OutcomeData> outcomes)
    {
        _helperContainer.style.display = DisplayStyle.None;
    }
}
