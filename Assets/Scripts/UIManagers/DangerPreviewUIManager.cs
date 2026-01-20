using System.Collections.Generic;
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
    List<ConsequencePreview> _consequences;

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

    private void OnNewDataIncoming(List<ConsequencePreview> consequences, List<DecisionData> decisions, VisualTreeAsset mapAsset)
    {
        _consequences = consequences;
    }

    private void OnIndicatorClick(DangerLevel dangerLevel)
    {
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
    }
}
