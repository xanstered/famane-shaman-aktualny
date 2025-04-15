using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemDescriptionUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject descriptionPanel;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI itemNameText;

    [Header("UI Styling")]
    public Color panelColor = new Color(0, 0, 0, 0.7f);
    public Color textColor = Color.white;
    public float fadeSpeed = 5f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        if (descriptionPanel != null)
        {
            canvasGroup = descriptionPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = descriptionPanel.AddComponent<CanvasGroup>();
            }

            HideDescription();
        }

        // Ustaw domyœlne kolory
        if (descriptionPanel != null)
        {
            UnityEngine.UI.Image panelImage = descriptionPanel.GetComponent<UnityEngine.UI.Image>();
            if (panelImage != null)
            {
                panelImage.color = panelColor;
            }
        }

        if (descriptionText != null)
        {
            descriptionText.color = textColor;
        }

        if (itemNameText != null)
        {
            itemNameText.color = textColor;
        }
    }

    public void ShowDescription(string itemName, string description)
    {
        if (descriptionPanel == null || canvasGroup == null) return;

        // Ustaw tekst
        if (itemNameText != null)
            itemNameText.text = itemName;

        if (descriptionText != null)
            descriptionText.text = description;

        // Poka¿ panel z animacj¹ fade-in
        descriptionPanel.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public void HideDescription()
    {
        if (descriptionPanel == null || canvasGroup == null) return;

        // Ukryj panel z animacj¹ fade-out
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    System.Collections.IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        descriptionPanel.SetActive(false);
    }
}
