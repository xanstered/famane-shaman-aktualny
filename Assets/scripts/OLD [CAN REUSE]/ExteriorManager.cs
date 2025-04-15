using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExteriorManager : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnPoint; // Punkt przed drzwiami
    [SerializeField] private GameObject player; // Referencja do gracza, kt�ry ju� jest na scenie
    [SerializeField] private GameObject sisterSprite;
    [SerializeField] private DialogueManager dialogueManager;

    void Start()
    {
        // Sprawd�, czy gracz w�a�nie wyszed� z chatki
        if (PlayerPrefs.GetInt("ShouldStartDialog", 0) == 1)
        {
            // Teleportuj istniej�cego gracza do punktu spawnu
            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;

            // Poka� siostr� i rozpocznij dialog
            StartDialog();

            // Wyczy�� flag�
            PlayerPrefs.DeleteKey("ShouldStartDialog");
        }
    }

    private void StartDialog()
    {
        sisterSprite.SetActive(true);
        DialogueManager.DialogueData dialogue = new DialogueManager.DialogueData
        {
            lines = new DialogueManager.DialogueLine[]
       {
        new DialogueManager.DialogueLine { speakerName = "Siostra", text = "Uwa�aj na siebie..." },
        new DialogueManager.DialogueLine { speakerName = "Siostra", text = "Ta chatka skrywa mroczne tajemnice..." }
       }
        };

        dialogueManager.StartDialogue(dialogue);
    }
}