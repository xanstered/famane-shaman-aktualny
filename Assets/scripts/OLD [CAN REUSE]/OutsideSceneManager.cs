using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideSceneManager : MonoBehaviour
{

    [Header("Dialogue Settings")]
    [SerializeField] private DialogueContainer outsideDialogue;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private float dialogueStartDelay = 0.5f;

    private void Start()
    {
        // Upewnij si�, �e wszystkie referencje s� poprawnie ustawione
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }

        // Rozpocznij dialog po kr�tkim op�nieniu
        Invoke("StartOutsideDialogue", dialogueStartDelay);
    }

    private void StartOutsideDialogue()
    {
        if (outsideDialogue != null && dialogueManager != null)
        {
            dialogueManager.StartDialogue(outsideDialogue.dialogueData);
        }
        else
        {
            Debug.LogError("Missing dialogue references in OutsideSceneManager!");
        }
    }
}