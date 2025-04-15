using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPushing : MonoBehaviour
{
    public float pushForce = 5f;
    Rigidbody rb;

    public AudioClip pushingSound;

    private AudioSource audioSource;

    private bool isPushing = false;
    private GameObject currentPushableObject = null;

    public float fadeOutDuration = 0.5f;
    private bool isFadingOut = false;
    private float initialVolume;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = pushingSound;
        audioSource.loop = true;
        audioSource.volume = 1f;
        audioSource.playOnAwake = false;

        initialVolume = audioSource.volume;
    }

    private void Update()
    {
        if (isPushing && currentPushableObject == null && !isFadingOut)
        {
            StartFadeOut();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pushable"))
        {
            currentPushableObject = collision.gameObject;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pushable"))
        {
            Rigidbody objectRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            if (objectRigidbody != null)
            {
                Vector3 pushDirection = collision.gameObject.transform.position - transform.position;
                pushDirection = pushDirection.normalized;

                objectRigidbody.AddForce(pushDirection * pushForce, ForceMode.Force);


                if (!isPushing && !isFadingOut && pushingSound != null)
                {
                    audioSource.volume = initialVolume;
                    isPushing = true;
                    currentPushableObject = collision.gameObject;
                    audioSource.Play();
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pushable") && collision.gameObject == currentPushableObject)
        {
            currentPushableObject = null;
            if (!isFadingOut)
            {
                StartFadeOut();
            }
        }
    }

    private void StartFadeOut()
    {
        if (isPushing && !isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutSound());
        }
    }

    private IEnumerator FadeOutSound()
    {
        float startVolume = audioSource.volume;
        float startTime = Time.time;
        float endTime = startTime + fadeOutDuration;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float percentComplete = elapsedTime / fadeOutDuration;

            audioSource.volume = Mathf.Lerp(startVolume, 0, percentComplete);

            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();

        audioSource.volume = initialVolume;
        isPushing = false;
        isFadingOut = false;
    }
}