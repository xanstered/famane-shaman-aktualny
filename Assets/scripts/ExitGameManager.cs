using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameManager : MonoBehaviour
{
    [Header("Panele")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject confirmationPanel;

    [Header("Panel Potwierdzenia")]
    [SerializeField] private Image darkOverlay;
    [SerializeField] private float overlayAlpha = 0.7f; 

    private void Start()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    public void ShowExitConfirmation()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(true);

        if (darkOverlay != null)
        {
            Color overlayColor = darkOverlay.color;
            overlayColor.a = overlayAlpha;
            darkOverlay.color = overlayColor;
        }
    }

    public void ConfirmExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void CancelExit()
    {
        // Ukryj panel potwierdzenia
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }
}
