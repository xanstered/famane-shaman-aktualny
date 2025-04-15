using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private float typingSpeed = 0.05f;

    private bool isTyping = false;
    private Coroutine typingCoroutine;

    // Referencja do aktualnego dialogu
    private DialogueData currentDialogue;
    private int currentLineIndex = 0;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    // Struktura dla pojedynczej linii dialogowej
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public string text;
    }

    // Klasa przechowuj¹ca ca³y dialog
    [System.Serializable]
    public class DialogueData
    {
        public DialogueLine[] lines;
    }

    public void StartDialogue(DialogueData dialogue)
    {
        Debug.Log("Starting dialogue");
        foreach (var line in dialogue.lines)
        {
            Debug.Log($"Speaker: {line.speakerName}, Text: {line.text}");
        }
        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        DisplayNextLine();
    }

    void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Jeœli tekst siê pisze, poka¿ go od razu ca³y
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    dialogueText.text = currentDialogue.lines[currentLineIndex].speakerName + ": "
                                      + currentDialogue.lines[currentLineIndex].text;
                    isTyping = false;
                }
            }
            else
            {
                DisplayNextLine();
            }
        }
    }

    private void DisplayNextLine()
    {
        if (currentLineIndex < currentDialogue.lines.Length)
        {
            typingCoroutine = StartCoroutine(TypeLine(currentDialogue.lines[currentLineIndex]));
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        dialogueText.text = "";
        // Upewnij siê, ¿e tekst jest prawid³owo formatowany
        Debug.Log($"Writing line: {line.speakerName}: {line.text}"); // Debug
        dialogueText.text = $"{line.speakerName}: {line.text}";
        isTyping = false;
        yield break;
    }

    public event System.Action OnDialogueEnd;

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        OnDialogueEnd?.Invoke();
    }
}