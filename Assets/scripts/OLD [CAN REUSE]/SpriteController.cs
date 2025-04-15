using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    private void LateUpdate()
    {
        // Billboard effect - sprite zawsze zwr�cony do kamery
        transform.forward = Camera.main.transform.forward;
    }
}