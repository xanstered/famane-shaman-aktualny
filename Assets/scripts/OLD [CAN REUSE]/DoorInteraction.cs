using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private string outsideSceneName = "Prolog";
    [SerializeField] private TextMeshProUGUI promptText;

    private Camera playerCamera;
    private bool canInteract = false;

    private void Start()
    {
        playerCamera = Camera.main;
        promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckDoorInteraction();
        HandleInteractionInput();
    }

    private void CheckDoorInteraction()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                ShowPrompt();
                canInteract = true;
            }
            else
            {
                HidePrompt();
                canInteract = false;
            }
        }
        else
        {
            HidePrompt();
            canInteract = false;
        }
    }

    private void HandleInteractionInput()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.F))
        {
            TransitionToOutside();
        }
    }

    private void ShowPrompt()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = "[F] go outside";
    }

    private void HidePrompt()
    {
        promptText.gameObject.SetActive(false);
    }

    private void TransitionToOutside()
    {
        SceneManager.LoadScene(outsideSceneName);
    }
}