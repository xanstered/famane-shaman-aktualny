using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseLevel : MonoBehaviour
{
    public void PlayLevel1 ()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void Hub ()
    {
        SceneManager.LoadScene("Hub");
    }

    public void PlayJotunheim ()
    {
        SceneManager.LoadScene("Jotunheim");
    }
}
