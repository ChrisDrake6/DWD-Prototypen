using UnityEngine;
using UnityEngine.UIElements;

public class SuggestionUIManager : MonoBehaviour
{
    public static SuggestionUIManager Instance { get; private set; }

    [SerializeField] private UIDocument ui;

    private Button _buttonApproved;
    private Button _buttonRejected;
    private Label _dangerInfo;
    private Label _description;
    private Label _costDisplay;
    private Label _lostDisplay;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _description = ui.rootVisualElement.Q<Label>("Description");
        _costDisplay = ui.rootVisualElement.Q<Label>("Cost");
        _lostDisplay = ui.rootVisualElement.Q<Label>("Lost");

        _buttonApproved = ui.rootVisualElement.Q<Button>("ButtonApproved");
        _buttonRejected = ui.rootVisualElement.Q<Button>("ButtonRejected");

        _buttonApproved.RegisterCallback<ClickEvent>(OnApprovedClick);
        _buttonRejected.RegisterCallback<ClickEvent>(OnRejectedClick);
    }

    private void OnDisable()
    {
        _buttonApproved.UnregisterCallback<ClickEvent>(OnApprovedClick);
        _buttonRejected.UnregisterCallback<ClickEvent>(OnRejectedClick);
    }

    public void FillUI(SuggestionData suggestionData)
    {
        _description.text = suggestionData.SuggestionDescription;
        _costDisplay.text = $"Erwartete Kosten: {suggestionData.Cost}€";
        _lostDisplay.text = $"Erwartete Schäden: {suggestionData.Lost}€";
    }

    private void OnApprovedClick(ClickEvent ev)
    {
        GameManager.Instance.SubmitSuggestionAnswer(true);
    }

    private void OnRejectedClick(ClickEvent ev)
    {
        GameManager.Instance.SubmitSuggestionAnswer(false);
    }
}
