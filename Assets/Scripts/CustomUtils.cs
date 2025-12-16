using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CustomUtils
{
    public static List<T> GetSOAssets<T>(string path) where T : ScriptableObject
    {
        List<T> assets = new List<T>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { path });
        foreach (string assetName in assetNames)
        {
            string pathToSO = AssetDatabase.GUIDToAssetPath(assetName);
            assets.Add(AssetDatabase.LoadAssetAtPath<T>(pathToSO));
        }
        return assets.Where(a => a != null).ToList();
    }
}
