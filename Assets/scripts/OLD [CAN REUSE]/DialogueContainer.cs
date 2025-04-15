using UnityEngine;
using UnityEditor;

// Definiujemy œcie¿kê w menu Create
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Container")]
public class DialogueContainer : ScriptableObject
{
    public DialogueManager.DialogueData dialogueData;
}

#if UNITY_EDITOR
// Klasa pomocnicza do tworzenia w edytorze
public class DialogueContainerCreator
{
    [MenuItem("Assets/Create/Dialogue System/Dialogue Container")]
    public static void CreateDialogueContainer()
    {
        var asset = ScriptableObject.CreateInstance<DialogueContainer>();
        AssetDatabase.CreateAsset(asset, "Assets/New Dialogue.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
#endif