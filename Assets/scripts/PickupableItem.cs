using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    public Sprite icon;

    public string itemName;
    public string itemDescription;
    public bool isKey = false;

    public virtual void UseItem()
    {
        Debug.Log("used item: " + itemName);
    }
}
