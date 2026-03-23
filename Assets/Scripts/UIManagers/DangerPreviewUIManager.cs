using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DangerPreviewUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument ui;
    [SerializeField] private VisualTreeAsset modalContainerAsset;
    [SerializeField] private VisualTreeAsset previewEntryAsset;

    private VisualElement _backGround;
    private VisualElement _modalContainer;
    private Button _buttonClose;
    ConsequencePreview[] _consequences;

    public static event Action WindowClosed;

    private void OnEnable()
    {
        _backGround = ui.rootVisualElement.Q<VisualElement>("BackGround");
        GameManager.RoundStarted += OnNewDataIncoming;
        WeatherMapUIManager.IndicatorClicked += OnIndicatorClick;
    }

    private void OnDisable()
    {
        GameManager.RoundStarted -= OnNewDataIncoming;
        WeatherMapUIManager.IndicatorClicked -= OnIndicatorClick;
    }

    /// <summary>
    /// Gets triggered when the gamemanager starts a new round.
    /// </summary>
    /// <param name="levelData"></param>
    private void OnNewDataIncoming(LevelContentContainer levelData)
    {
        _consequences = levelData.Consequences;
    }

    /// <summary>
    /// Gets triggered, if player clicks on one of the three knobs on the weathermap.
    /// Build and fill a modal window with profiles and texts regarding past and current experiences.
    /// </summary>
    /// <param name="dangerLevel"></param>
    private void OnIndicatorClick(DangerLevel dangerLevel)
    {
        // Pick the correct entry corresponding to the button clicked.
        ConsequencePreview currentPreview = _consequences.FirstOrDefault(a => a.DangerLevel == dangerLevel);

        _modalContainer = modalContainerAsset.Instantiate().Q<VisualElement>("ModalContainer");
        _backGround.Add(_modalContainer);

        _buttonClose = _modalContainer.Q<Button>("Button_Cancel");
        _buttonClose.clicked += OnClosePreviewClick;

        ScrollView previewList = _modalContainer.Q<ScrollView>("EntryList");

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
        WindowClosed.Invoke();
    }
}
