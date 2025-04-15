using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySystem : MonoBehaviour
{

    public GameObject[] inventorySlots = new GameObject[4];
    private GameObject[] itemsInInventory = new GameObject[4];

    public float pickupRange = 3.0f;
    public LayerMask pickupLayer;
    private Camera playerCamera;

    private int selectedSlotIndex = -1;
    private float selectionTimer = 0f;
    private float selectionTimeout = 3.0f;

    public TextMeshProUGUI promptText;

    private bool isLookingAtPickupItem = false;

    void Start()
    {
        playerCamera = Camera.main;

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckLookTarget();

        HandleKeyInput();

        UpdateSelectionTimer();
    }

    void CheckLookTarget()
    {
        RaycastHit hit;
        isLookingAtPickupItem = false;

        if (selectedSlotIndex != -1)
            return;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupRange, pickupLayer))
        {
            if (hit.collider.CompareTag("Pickupable"))
            {
                isLookingAtPickupItem = true;
                string itemName = GetItemName(hit.collider.gameObject);

                promptText.text = "Press [F] to pick up " + itemName;
                promptText.gameObject.SetActive(true);

                // Jeœli naciœniêto F, podnieœ przedmiot
                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickupItem(hit.collider.gameObject);
                }
            }
        }
        else if (promptText.gameObject.activeSelf && !promptText.text.Contains("drop/use"))
        {
            promptText.gameObject.SetActive(false);
        }
    }

    void HandleKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectInventorySlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectInventorySlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectInventorySlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SelectInventorySlot(3);

        // obslugiwanie klawisza F dla uzycia/upuszczania wybranego itemka
        if (Input.GetKeyDown(KeyCode.F) && selectedSlotIndex != -1)
        {
            UseOrDropItem(selectedSlotIndex);
            selectedSlotIndex = -1; // reset wyboru
        }
    }

    void SelectInventorySlot(int slotIndex)
    {
        Debug.Log("chosen slot: " + (slotIndex + 1));

        if (itemsInInventory[slotIndex] == null)
        {
            if (selectedSlotIndex != -1)
            {
                selectedSlotIndex = -1;
                promptText.gameObject.SetActive(false);
            }
            return;
        }

        selectedSlotIndex = slotIndex;
        selectionTimer = 0f; // resecik timera

        string itemName = GetItemName(itemsInInventory[slotIndex]);

        promptText.text = "Press [F] to drop/use " + itemName;
        promptText.gameObject.SetActive(true);

        Debug.Log("wyswietlam tekst: " + promptText.text);
    }

    void UpdateSelectionTimer()
    {
        if (selectedSlotIndex != -1)
        {
            selectionTimer += Time.deltaTime;

            if (selectionTimer >= selectionTimeout)
            {
                Debug.Log("uplynal czas wiec reset wyboru");
                selectedSlotIndex = -1;

                if (!isLookingAtPickupItem)
                {
                    promptText.gameObject.SetActive(false);
                }
            }
        }
    }

    void PickupItem(GameObject item)
    {
        Debug.Log("probujemy podniesc przedmiot: " + item.name);

        //znajdujemy pierwszy wolny slocik
        for (int i = 0; i < itemsInInventory.Length; i++)
        {
            if (itemsInInventory[i] == null)
            {
                AddItemToInventory(item, i);
                promptText.gameObject.SetActive(false);
                return;
            }
        }

        Debug.Log("Ekwipunek pe³ny!");
    }

    void UseOrDropItem(int slotIndex)
    {
        if (itemsInInventory[slotIndex] != null)
        {
            Debug.Log("U¿ywam/upuszczam przedmiot ze slotu " + (slotIndex + 1));

            //TUTAJ MOZE BYC LOGIKA UZYWANIA ITEMKOW

            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupRange))
            {
                DoorController door = hit.collider.GetComponent<DoorController>();
                if (door != null)
                {
                    door.UseKeyOnDoor(itemsInInventory[slotIndex]);
                    return;
                }
            }

            DropItem(slotIndex);

            promptText.gameObject.SetActive(false);
        }
    }

    string GetItemName(GameObject item)
    {
        if (item != null)
        {
            PickupableItem pickupItem = item.GetComponent<PickupableItem>();
            if (pickupItem != null && !string.IsNullOrEmpty(pickupItem.itemName))
                return pickupItem.itemName;
            return item.name;
        }
        return "item";
    }

    void AddItemToInventory(GameObject item, int slotIndex)
    {
        Debug.Log($"dodaje item {item.name} do slotu {slotIndex}");

        itemsInInventory[slotIndex] = item;

        item.SetActive(false);

        UpdateInventoryUI(slotIndex, item);
    }

    public void RemoveItemFromInventory(GameObject item)
    {
        for (int i = 0; i < itemsInInventory.Length; i++)
        {
            if (itemsInInventory[i] == item)
            {
                itemsInInventory[i] = null;

                ClearInventorySlotUI(i);

                Debug.Log($"Usuniêto {item.name} z ekwipunku");
                return;
            }
        }
    }


        void DropItem(int slotIndex)
        {
            Debug.Log($"upuszczanie itemka ze slotu {slotIndex}");

            GameObject item = itemsInInventory[slotIndex];

            item.SetActive(true);

            item.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 2.0f;

            // dodanie sily do itemka jesli ma rb
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(playerCamera.transform.forward * 2.0f, ForceMode.Impulse);
            }

            itemsInInventory[slotIndex] = null;

            ClearInventorySlotUI(slotIndex);
        }

        void UpdateInventoryUI(int slotIndex, GameObject item)
        {
            PickupableItem pickupableItem = item.GetComponent<PickupableItem>();

            if (pickupableItem != null && pickupableItem.icon != null && inventorySlots[slotIndex] != null)
            {
                Image slotImage = inventorySlots[slotIndex].GetComponentInChildren<Image>();
                if (slotImage != null)
                {
                    slotImage.sprite = pickupableItem.icon;
                    slotImage.enabled = true;
                }
            }
        }

        void ClearInventorySlotUI(int slotIndex)
        {
            if (inventorySlots[slotIndex] != null)
            {
                Image slotImage = inventorySlots[slotIndex].GetComponentInChildren<Image>();
                if (slotImage != null)
                {
                    slotImage.sprite = null;
                    slotImage.enabled = false;
                }
            }
        }
}