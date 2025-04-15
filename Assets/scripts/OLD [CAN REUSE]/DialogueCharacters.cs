using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCharacters : MonoBehaviour
{
    [SerializeField] private GameObject sisterObject;
    [SerializeField] private GameObject oldManObject;

    private void Start()
    {
        sisterObject.SetActive(false);
        oldManObject.SetActive(false);
    }

    public void ShowCharacters()
    {
        sisterObject.SetActive(true);
        oldManObject.SetActive(true);
    }

    public void HideCharacters()
    {
        sisterObject.SetActive(false);
        oldManObject.SetActive(false);
    }
}