using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDoorSound : MonoBehaviour
{
    public AudioClip doorCloseSound;

    // Start is called before the first frame update
    void Start()
    {
        if (doorCloseSound != null)
        {
            AudioSource.PlayClipAtPoint(doorCloseSound, Camera.main.transform.position);
        }
        else
        {
            Debug.LogWarning("door close sound not assigned");
        }
    }
}
