using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument ui;

    private Button _buttonNewGame;

    void Start()
    {
        _buttonNewGame = ui.rootVisualElement.Q<Button>("Button_NewGame");
        _buttonNewGame.clicked += StartGame;
    }

    private void OnDestroy()
    {
        _buttonNewGame.clicked -= StartGame;
    }

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
