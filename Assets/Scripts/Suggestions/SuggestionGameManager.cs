using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SuggestionGameManager : MonoBehaviour
{
    [SerializeField] private int SuggestionAmount = 4;

    public static event Action<SuggestionData> NewSuggestionPicked;
    public static event Action<bool> ResultCalculated;
    public static event Action<SuggestionResultData> GameOver;

    private List<SuggestionData> _chosenSuggestions;
    private SuggestionResultData _suggestionResultData;
    private int _suggestionIndex;

    private void OnEnable()
    {
        SuggestionUIManager.DescisionMade += OnSuggestionAnswerSubmitted;
        SuggestionUIManager.NextSuggestionCalled += OnNextSuggestionCalled;

        _suggestionResultData = new SuggestionResultData();

        _chosenSuggestions = new List<SuggestionData>();
        List<SuggestionData> allSuggestions = new List<SuggestionData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { Path.Combine("Assets", "Suggestions") });
        foreach (string assetName in assetNames)
        {
            string pathToSO = AssetDatabase.GUIDToAssetPath(assetName);
            allSuggestions.Add(AssetDatabase.LoadAssetAtPath<SuggestionData>(pathToSO));
        }

        for (int i = 0; i < SuggestionAmount; i++)
        {
            GetRandomSuggestion(allSuggestions);
        }

        NewSuggestionPicked.Invoke(_chosenSuggestions[_suggestionIndex]);
    }

    private void OnDisable()
    {
        SuggestionUIManager.DescisionMade -= OnSuggestionAnswerSubmitted;
        SuggestionUIManager.NextSuggestionCalled -= OnNextSuggestionCalled;
    }

    private void OnSuggestionAnswerSubmitted(bool approved)
    {
        bool eventOccured;

        float diceRoll = Random.Range(0f, 100f);
        eventOccured = _chosenSuggestions[_suggestionIndex].DangerPropability <= diceRoll;

        if (approved)
        {
            _suggestionResultData.CostSum += _chosenSuggestions[_suggestionIndex].Cost;
            if (eventOccured)
            {
                _suggestionResultData.HappinessLevel += _chosenSuggestions[_suggestionIndex].PositiveHappinessImpact;
            }
            else
            {
                _suggestionResultData.HappinessLevel -= _chosenSuggestions[_suggestionIndex].UnneccessaryActionHappinessImpact;
            }
        }
        else
        {
            if (eventOccured)
            {
                _suggestionResultData.LostSum += _chosenSuggestions[_suggestionIndex].Lost;
                _suggestionResultData.HappinessLevel -= _chosenSuggestions[_suggestionIndex].NegativeHappinessImpact;
            }
        }
        _suggestionResultData.HappinessLevel = Math.Clamp(_suggestionResultData.HappinessLevel, 0, 100);

        ResultCalculated.Invoke(eventOccured);
    }

    private void OnNextSuggestionCalled()
    {
        _suggestionIndex++;
        if (_suggestionIndex == _chosenSuggestions.Count)
        {
            GameOver.Invoke(_suggestionResultData);
        }
        else
        {
            NewSuggestionPicked.Invoke(_chosenSuggestions[_suggestionIndex]);
        }
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