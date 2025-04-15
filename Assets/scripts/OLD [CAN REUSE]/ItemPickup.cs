using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Podstawowe ustawienia")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private Transform itemHolder;
    [SerializeField] private Vector3 heldItemPosition = new Vector3(0, -0.5f, 2f);

    [Header("Drewno")]
    [SerializeField] private GameObject woodPrefab; // Prefab drewna które bêdziemy dostawaæ

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI promptText;

    private GameObject currentItem;
    private GameObject heldItem;
    private bool isHoldingAxe = false;
    private bool isHoldingBucket = false;
    private bool isBucketFilled = false; // Czy wiadro jest nape³nione wod¹?

    private void Start()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (heldItem == null)
        {
            CheckForInteractable();
        }
        else
        {
            CheckForWellInteraction();

            if (promptText != null)
            {
                promptText.text = $"[G] Drop: " + heldItem.name;
                promptText.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                DropItem();
            }
        }
    }

    private void CheckForInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange))
        {
            // Sprawdzanie przedmiotu do podniesienia
            if (hit.collider.CompareTag("Pickupable"))
            {
                if (promptText != null)
                {
                    promptText.text = "[F] Pick up";
                    promptText.gameObject.SetActive(true);
                }

                currentItem = hit.collider.gameObject;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickupItem();
                }
            }
            // Sprawdzanie drzewa gdy trzymamy siekierê
            else if (hit.collider.CompareTag("Tree") && isHoldingAxe)
            {
                if (promptText != null)
                {
                    promptText.text = "[E] Cut tree";
                    promptText.gameObject.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    CutTree(hit.collider.gameObject);
                }
            }
            else
            {
                if (promptText != null)
                {
                    promptText.gameObject.SetActive(false);
                }
                currentItem = null;
            }
        }
        else
        {
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
            currentItem = null;
        }
    }

    private void CutTree(GameObject tree)
    {
        // Upuszczamy siekierê
        DropItem();
        isHoldingAxe = false;

        // Ukrywamy lub niszczymy drzewo
        tree.SetActive(false); // lub Destroy(tree);

        // Tworzymy i podnosimy drewno
        if (woodPrefab != null)
        {
            GameObject wood = Instantiate(woodPrefab, tree.transform.position, Quaternion.identity);
            currentItem = wood;
            PickupItem();
        }
    }

    private void CheckForWellInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Well") && IsHoldingBucket() && !isBucketFilled)
            {
                if (promptText != null)
                {
                    promptText.text = "[F] Fill with water";
                    promptText.gameObject.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    FillBucket();
                }
            }
        }
    }

    private void PickupItem()
    {
        if (currentItem != null)
        {
            currentItem.transform.SetParent(itemHolder);
            currentItem.transform.localPosition = heldItemPosition;
            currentItem.transform.localRotation = Quaternion.identity;

            if (currentItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            if (currentItem.TryGetComponent<Collider>(out Collider collider))
            {
                collider.enabled = false;
            }

            heldItem = currentItem;

            // Sprawdzamy, czy podnieœliœmy siekierê
            isHoldingAxe = heldItem.GetComponent<Axe>() != null;
            isBucketFilled = heldItem.name.Contains("Filled");


            Debug.Log("Podniesiono przedmiot: " + heldItem.name + ", isHoldingAxe: " + isHoldingAxe);
            currentItem = null;
        }
    }

    private void FillBucket()
    {
        isBucketFilled = true;
        heldItem.name = "Bucket (Filled)";

        if (promptText != null)
        {
            promptText.text = "Wiadro nape³nione wod¹!";
        }

        Debug.Log("Wiadro zosta³o nape³nione wod¹!");
    }


    private void DropItem()
    {
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null);

            if (heldItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddForce(Camera.main.transform.forward * 5f, ForceMode.Impulse);
            }

            if (heldItem.TryGetComponent<Collider>(out Collider collider))
            {
                collider.enabled = true;
            }

            if (heldItem.GetComponent<Axe>() != null)
            {
                isHoldingAxe = false;
            }

            heldItem = null;

            isHoldingBucket = false;
            isBucketFilled = false;

            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private bool IsHoldingBucket()
    {
        return heldItem != null && heldItem.name.Contains("Bucket");
    }
}