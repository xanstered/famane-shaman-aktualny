using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDescription : MonoBehaviour
{
    [TextArea(3, 10)]
    public string description = "Description";
    public string itemName = "Item";
}
