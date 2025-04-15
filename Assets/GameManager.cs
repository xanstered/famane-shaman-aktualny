using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private GameObject introText;

    [Header("Dialogue")]
    [SerializeField] private DialogueContainer initialDialogue;
    [SerializeField] private DialogueManager dialogueManager;


    [Header("Characters")]
    [SerializeField] private GameObject sisterObject;
    [SerializeField] private GameObject oldManObject;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject bg;

    private bool introShown = true;

    void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();
        dialogueManager.OnDialogueEnd += HideCharacters;
        sisterObject.SetActive(false);
        oldManObject.SetActive(false);
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (introShown && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartGameSequence());
        }
    }

    private IEnumerator StartGameSequence()
    {
        introShown = false;
        introText.SetActive(false);

        yield return null;

        sisterObject.SetActive(true);
        oldManObject.SetActive(true);
        dialoguePanel.SetActive(true);
        bg.SetActive(false);

        // Rozpocznij dialog u¿ywaj¹c stworzonego kontenera
        dialogueManager.StartDialogue(initialDialogue.dialogueData);
    }

    private void HideCharacters()
    {
        sisterObject.SetActive(false);
        oldManObject.SetActive(false);
    }
}