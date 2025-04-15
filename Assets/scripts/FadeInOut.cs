using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("Panel z obrazem, który bêdzie zaciemniaæ ekran")]
    public Image fadePanel;

    [Tooltip("Domyœlny czas trwania efektu fade w sekundach")]
    public float defaultFadeDuration = 1.0f;

    [Tooltip("Czy panel powinien byæ przeŸroczysty na starcie")]
    public bool startTransparent = true;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        InitializeFadePanel();

        Debug.Log("FadeInOut Start wywo³ane");

        if (fadePanel == null)
        {
            Debug.LogError("nie przypisano panelu do wyciemnienia ekranu");

            fadePanel = GetComponentInChildren<Image>();
            if (fadePanel != null)
            {
                Debug.Log("Automatycznie znaleziono panel w dzieciach");
            }
            else
            {
                Image[] allImages = FindObjectsOfType<Image>();
                Debug.Log($"Znaleziono {allImages.Length} obrazów w scenie");
                return;
            }
        }
        else
        {
            Debug.Log("FadePanel znaleziony: " + fadePanel.name);
        }

        Color initialColor = fadePanel.color;
        initialColor.a = startTransparent ? 0f : 1f;
        fadePanel.color = initialColor;

        fadePanel.gameObject.SetActive(true);
    }

    public void FadeIn(float duration = -1f)
    {
        float fadeDuration = duration > 0f ? duration : defaultFadeDuration;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeRoutine(0f, 1f, fadeDuration));
    }

    public void InitializeFadePanel()
    {
        if (fadePanel == null)
        {
            fadePanel = GetComponentInChildren<Image>();

            if (fadePanel == null)
            {
                Image[] allImages = FindObjectsOfType<Image>();
                foreach (Image img in allImages)
                {
                    if (img.name.ToLower().Contains("fade") || img.name.ToLower().Contains("black"))
                    {
                        fadePanel = img;
                        Debug.Log("Znaleziono panel przez nazwê: " + img.name);
                        break;
                    }
                }
            }
        }

        if (fadePanel != null)
        {
            Color currentColor = fadePanel.color;
            currentColor.a = startTransparent ? 0f : 1f;
            fadePanel.color = currentColor;
            fadePanel.gameObject.SetActive(true);
            Debug.Log("Panel zainicjalizowany poprawnie");
        }
        else
        {
            Debug.LogError("Nadal nie znaleziono panelu - stworzê nowy");
        }
    }



    public void FadeOut(float duration = -1f)
    {
        float fadeDuration = duration > 0 ? duration : defaultFadeDuration;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeRoutine(1f, 0f, fadeDuration));
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha, float duration)
    {
        if (fadePanel == null) yield break;

        Color currentColor = fadePanel.color;
        currentColor.a = startAlpha;
        fadePanel.color = currentColor;

        fadePanel.gameObject.SetActive(true);

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime; 
            float normalizedTime = Mathf.Clamp01(timeElapsed / duration);

            currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            fadePanel.color = currentColor;

            yield return null;
        }

        currentColor.a = targetAlpha;
        fadePanel.color = currentColor;

        if (targetAlpha == 0f)
        {
            fadePanel.gameObject.SetActive(false);
        }

        fadeCoroutine = null;
    }
}
