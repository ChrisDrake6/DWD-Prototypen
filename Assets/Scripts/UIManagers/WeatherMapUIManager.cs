using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WeatherMapUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    
    public static event Action<DangerLevel> IndicatorClicked;
    public static event Action DecisionButtonClicked;

    private VisualElement _backGround;
    private VisualElement _weatherMap;
    private Button _buttonDecision;
    private List<Button> _indicatorButtons;

    void OnEnable()
    {
        GameManager.RoundStarted += OnNewDataIncoming;
        GameManager.OutcomeCalculated += RemoveWeatherMap;
        GameManager.LastDecisionMade += RemoveWeatherMap;
        _backGround = ui.rootVisualElement.Q<VisualElement>("BackGround");
    }

    private void OnDisable()
    {        
        GameManager.RoundStarted -= OnNewDataIncoming;
        GameManager.OutcomeCalculated -= RemoveWeatherMap;
        GameManager.LastDecisionMade -= RemoveWeatherMap;
    }

    private void OnNewDataIncoming(LevelContentContainer levelData)
    {
        _weatherMap = levelData.LevelParameters.MapAsset.Instantiate().Q<VisualElement>("WeatherMap");
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
        ClearModal();
        IndicatorClicked.Invoke(DangerLevel.high);
    }
    private void OnMediumDangerButtonClicked()
    {
        ClearModal();
        IndicatorClicked.Invoke(DangerLevel.medium);
    }
    private void OnLowDangerButtonClicked()
    {
        ClearModal();
        IndicatorClicked.Invoke(DangerLevel.low);
    }
    
    private void OnShowDecisionsClick()
    {
        ClearModal();
        DecisionButtonClicked.Invoke();
    }
    
    private void RemoveWeatherMap(OutcomeData outcome)
    {
        RemoveWeatherMap(new List<OutcomeData>());
    }
    private void RemoveWeatherMap(List<OutcomeData> outcomes)
    {
        ClearModal();
        if (_backGround.Q<VisualElement>("WeatherMap") != null)
        {
            _backGround.Remove(_weatherMap);
            _indicatorButtons[0].clicked -= OnHighDangerButtonClicked;
            _indicatorButtons[1].clicked -= OnMediumDangerButtonClicked;
            _indicatorButtons[2].clicked -= OnLowDangerButtonClicked;
            _buttonDecision.clicked -= OnShowDecisionsClick;
        }
    }

    private void ClearModal()
    {
        VisualElement modalContainer = _backGround.Q<VisualElement>("ModalContainer");
        if(modalContainer != null)
        {
            _backGround.Remove(modalContainer);
        }
    }
}
