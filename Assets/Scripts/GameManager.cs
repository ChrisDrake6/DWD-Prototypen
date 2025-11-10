using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int SuggestionAmount = 4;

    private List<SuggestionData> _chosenSuggestions;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _chosenSuggestions = new List<SuggestionData>();
        List<SuggestionData> allSuggestions = new List<SuggestionData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { Path.Combine("Assets", "Suggestions") });
        foreach (string assetName in assetNames)
        {
            string pathToSO = AssetDatabase.GUIDToAssetPath(assetName);
            allSuggestions.Add(AssetDatabase.LoadAssetAtPath<SuggestionData>(pathToSO));
        }

        for(int i = 0; i < SuggestionAmount; i++)
        {
            GetRandomSuggestion(allSuggestions);
        }

        SuggestionUIManager.Instance.FillUI(_chosenSuggestions[0]);
    }

    public void SubmitSuggestionAnswer(bool approved)
    {
        Debug.Log(approved);
    }

    private void GetRandomSuggestion(List<SuggestionData> suggestions)
    {
        int randomIndex = Random.Range(0, suggestions.Count);
        if (_chosenSuggestions.Contains(suggestions[randomIndex]))
        {
            GetRandomSuggestion(suggestions);
        }
        else
        {
            _chosenSuggestions.Add(suggestions[randomIndex]);
        }
    }
}