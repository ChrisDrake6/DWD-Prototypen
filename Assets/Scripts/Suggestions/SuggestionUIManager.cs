using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SuggestionUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    [SerializeField] private VisualTreeAsset suggestionItemTemplate;
    [SerializeField] private VisualTreeAsset suggestionResultTemplate;
    [SerializeField] private VisualTreeAsset endResultTemplate;
    [SerializeField] private VisualTreeAsset informationDisplayItemTemplate;

    [SerializeField] private string textApprovedNotOccured;
    [SerializeField] private string textApprovedOccured;
    [SerializeField] private string textNotApprovedNotOccured;
    [SerializeField] private string textNotApprovedOccured;

    public static event Action<bool> DescisionMade;
    public static event Action NextSuggestionCalled;

    private VisualElement _backGround;

    // Suggestion Item
    private VisualElement _suggestionItem;
    private Button _buttonApproved;
    private Button _buttonRejected;
    private Button _buttonShowInfo;

    // Mapinfo
    private VisualElement _mapInfoContainer;
    private VisualElement _mapImageContainer;
    private Button _buttonHideInfo;
    private Button _buttonImageBack;
    private Button _buttonImageForward;

    // Result
    private VisualElement _suggestionResult;
    private Button _buttonContinue;
    private Label _resultText;

    // EndResult
    private VisualElement _endResult;
    private Button _buttonRestart;
    private Label _displayCostSum;
    private Label _displayLostSum;
    private Label _displayHappinessSum;

    private SuggestionData _suggestionData;
    private int _currentMapIndex;
    private bool _approved;

    private void OnEnable()
    {
        _backGround = ui.rootVisualElement.Q<VisualElement>("BackGround");
        SuggestionGameManager.NewSuggestionPicked += OnNewDataIncoming;
        SuggestionGameManager.ResultCalculated += OnIncomingResult;
        SuggestionGameManager.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        SuggestionGameManager.NewSuggestionPicked -= OnNewDataIncoming;
        SuggestionGameManager.ResultCalculated -= OnIncomingResult;
        SuggestionGameManager.GameOver -= OnGameOver;
    }

    private void OnNewDataIncoming(SuggestionData data)
    {
        _suggestionData = data;
        _currentMapIndex = 0;
        InstantiateSuggestionItem();
        _suggestionItem.dataSource = _suggestionData;
    }

    private void InstantiateSuggestionItem()
    {
        _suggestionItem = suggestionItemTemplate.Instantiate().Q<VisualElement>("SuggestionContainer");
        _backGround.Add(_suggestionItem);

        _buttonApproved = _suggestionItem.Q<Button>("ButtonApproved");
        _buttonRejected = _suggestionItem.Q<Button>("ButtonRejected");
        _buttonShowInfo = _suggestionItem.Q<Button>("ButtonMaps");

        _buttonApproved.clicked += OnApprovedClick;
        _buttonRejected.clicked += OnRejectedClick;
        _buttonShowInfo.clicked += OnShowInfoClick;
    }

    private void InstantiateResultScreen()
    {
        _suggestionResult = suggestionResultTemplate.Instantiate().Q<VisualElement>("ResultContainer");
        _backGround.Add(_suggestionResult);

        _buttonContinue = _suggestionResult.Q<Button>("ButtonContinue");
        _resultText = _suggestionResult.Q<Label>("ResultDisplay");

        _buttonContinue.clicked += OnContinueClick;
    }

    private void RemoveCurrentSuggestionItem()
    {
        _backGround.Remove(_suggestionItem);

        _buttonApproved.clicked -= OnApprovedClick;
        _buttonRejected.clicked -= OnRejectedClick;
        _buttonShowInfo.clicked -= OnShowInfoClick;
    }
    private void RemoveResultScreen()
    {
        _backGround.Remove(_suggestionResult);

        _buttonContinue.clicked -= OnContinueClick;
    }

    private void OnApprovedClick()
    {
        _approved = true;
        DescisionMade.Invoke(true);
    }

    private void OnRejectedClick()
    {
        _approved = false;
        DescisionMade.Invoke(false);
    }

    private void OnIncomingResult(bool eventOccured)
    {
        RemoveCurrentSuggestionItem();

        InstantiateResultScreen();
        _suggestionResult.dataSource = _suggestionData;

        if(_approved && eventOccured)
        {
            _resultText.text = textApprovedOccured;
        }
        else if(_approved && !eventOccured) 
        {
            _resultText.text = textApprovedNotOccured;
        }
        else if (!_approved && eventOccured)
        {
            _resultText.text = textNotApprovedOccured;
        }
        else if (!_approved && !eventOccured)
        {
            _resultText.text = textNotApprovedNotOccured;
        }
    }

    private void OnGameOver(SuggestionResultData data)
    {
        _endResult = endResultTemplate.Instantiate().Q<VisualElement>("EndResultContainer");
        _backGround.Add(_endResult);
        _buttonRestart = _endResult.Q<Button>("ButtonRestart");
        _displayCostSum = _endResult.Q<Label>("DisplayCost");
        _displayLostSum = _endResult.Q<Label>("DisplayLost");
        _displayHappinessSum = _endResult.Q<Label>("DisplayHappiness");

        _displayCostSum.text = data.CostSum.ToString() + " €";
        _displayLostSum.text = data.LostSum.ToString() + " €";
        _displayHappinessSum.text = data.HappinessLevel.ToString() + " %";

        _buttonRestart.clicked += OnRestartClick;
    }

    private void OnShowInfoClick()
    {
        _mapInfoContainer = informationDisplayItemTemplate.Instantiate().Q<VisualElement>("WeatherMapContainer");
        _backGround.Add(_mapInfoContainer);

        _mapImageContainer = _mapInfoContainer.Q<VisualElement>("MapDisplay");
        _buttonHideInfo = _mapInfoContainer.Q<Button>("ButtonCloseMap");
        _buttonImageBack = _mapInfoContainer.Q<Button>("ImageBack");
        _buttonImageForward = _mapInfoContainer.Q<Button>("ImageForward");

        _buttonHideInfo.clicked += OnHideInfoCLick;
        _buttonImageBack.RegisterCallback<ClickEvent>(OnChangeImageClick);
        _buttonImageForward.RegisterCallback<ClickEvent>(OnChangeImageClick);

        DisplayImage();
    }

    private void OnHideInfoCLick()
    {
        _buttonHideInfo.clicked -= OnHideInfoCLick;
        _buttonImageBack.UnregisterCallback<ClickEvent>(OnChangeImageClick);
        _buttonImageForward.UnregisterCallback<ClickEvent>(OnChangeImageClick);
        _backGround.Remove(_mapInfoContainer);
    }

    private void OnChangeImageClick(ClickEvent ev)
    {
        if (ev.target == _buttonImageBack)
        {
            _currentMapIndex--;
        }
        else
        {
            _currentMapIndex++;
        }

        DisplayImage();
    }

    private void DisplayImage()
    {
        if (_currentMapIndex == 0)
        {
            _buttonImageBack.style.display = DisplayStyle.None;
        }
        else
        {
            _buttonImageBack.style.display = DisplayStyle.Flex;
        }
        if (_currentMapIndex == _suggestionData.Maps.Length - 1)
        {
            _buttonImageForward.style.display = DisplayStyle.None;
        }
        else
        {
            _buttonImageForward.style.display = DisplayStyle.Flex;
        }

        _mapImageContainer.style.backgroundImage = new StyleBackground(_suggestionData.Maps[_currentMapIndex]);
    }

    private void OnContinueClick()
    {
        RemoveResultScreen();
        NextSuggestionCalled.Invoke();
    }

    private void OnRestartClick()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
