using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsequencePreview", menuName = "Scriptable Objects/ConsequencePreview")]
public class ConsequencePreview : ScriptableObject
{
    public DangerLevel DangerLevel;
    public ConsequencePreviewEntry[] entries;
}

[Serializable]
public class ConsequencePreviewEntry
{
    public Sprite ProfileImage;
    [TextArea(10, 15)]
    public string Text;
}
