using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HoleNextScene : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private bool isMenuScene = false;

    public TextMeshProUGUI promptText;

    private bool playerLooking = false;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckIfPlayerLookingAtHole();

        if (playerLooking && Input.GetKeyDown(KeyCode.F))
        {
            if (nextSceneName.ToLower().Contains("menu") || isMenuScene)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void CheckIfPlayerLookingAtHole()
    {
        if (mainCamera == null)
            return;

        RaycastHit hit;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        // SprawdŸ, czy raycast z kamery trafia w ten obiekt (dziurê)
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                // Gracz patrzy na dziurê
                playerLooking = true;

                // Poka¿ tekst podpowiedzi
                if (promptText != null)
                    promptText.gameObject.SetActive(true);

                return;
            }
        }

        playerLooking = false;
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

}
