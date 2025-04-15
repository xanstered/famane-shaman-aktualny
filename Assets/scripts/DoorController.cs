using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    FadeInOut fade;

    public Transform teleportDestination;
    public float interactionDistance = 3.0f;
    public LayerMask doorLayer;

    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    private AudioSource audioSource;

    private InventorySystem playerInventory;
    private Camera playerCamera;
    private CharacterController playerCharacterController;

    public TextMeshProUGUI promptText;

    private bool isLookingAtDoor = false;

    // Start is called before the first frame update
    void Start()
    {

        fade = FindObjectOfType<FadeInOut>();
        if (fade == null)
        {
            Debug.LogError("nie znaleziono komponentu FadeInOut w scenie");
        }

        playerInventory = FindAnyObjectByType<InventorySystem>();
        playerCamera = Camera.main;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCharacterController = player.GetComponent<CharacterController>();
            if (playerCharacterController == null)
            {
                Debug.LogWarning("nie ma characterController na obiekcie gracza");
            }
        }
        else
        {
            Debug.LogWarning("nie znaleziono obiektu gracza z tagiem 'player'");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;

        if (playerInventory == null)
        {
            Debug.LogError("cant find InventorySystem component");
        }

        gameObject.layer = LayerMask.NameToLayer("pickupLayer");

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool wasLookingAtDoor = isLookingAtDoor;
        isLookingAtDoor = CheckDoorInteraction();

        // Ukryj prompt, jeœli przestaliœmy patrzeæ na drzwi
        if (wasLookingAtDoor && !isLookingAtDoor)
        {
            if (playerInventory != null && playerInventory.promptText != null)
            {
                playerInventory.promptText.gameObject.SetActive(false);
            }
        }
    }

    bool CheckDoorInteraction()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance, doorLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (playerInventory != null && playerInventory.promptText != null)
                {
                    playerInventory.promptText.text = "locked";
                    playerInventory.promptText.gameObject.SetActive(true);
                }

                CheckKeyUsage();
                return true;
            }
        }

        return false;
    }

    void CheckKeyUsage()
    {

    }

    public void UseKeyOnDoor(GameObject keyItem)
    {
        PickupableItem pickupableItem = keyItem.GetComponent<PickupableItem>();

        if (pickupableItem != null && pickupableItem.isKey)
        {
            Debug.Log("U¿ywam klucza na drzwiach");

            if (playerInventory != null)
            {
                // Usuñ klucz z ekwipunku
                playerInventory.RemoveItemFromInventory(keyItem);


                if (playerInventory.promptText != null)
                {
                    playerInventory.promptText.gameObject.SetActive(false);
                }
            }

            if (doorOpenSound != null)
            {
                AudioSource.PlayClipAtPoint(doorOpenSound, transform.position);
            }

            StartCoroutine(TeleportPlayerWithDelay());
        }
        else
        {
            Debug.Log("this is not a key");
            if (playerInventory != null && playerInventory.promptText != null)
            {
                playerInventory.promptText.text = "locked";
            }
        }
    }


    System.Collections.IEnumerator TeleportPlayerWithDelay()
    {
        if (fade != null)
        {
            fade.FadeIn();
        }

        yield return new WaitForSeconds(1f);

        if (teleportDestination != null && playerCharacterController != null)
        {
            playerCharacterController.enabled = false;

            // teleport player
            playerCharacterController.transform.position = teleportDestination.position;
            playerCharacterController.transform.rotation = teleportDestination.rotation;

            playerCharacterController.enabled = true;

            if (doorCloseSound != null)
            {
                AudioSource.PlayClipAtPoint(doorCloseSound, teleportDestination.position);
            }

            Debug.Log("player teleported");

            yield return new WaitForSeconds(0.3f);

            if (fade != null)
            {
                fade.FadeOut();
            }
        }
        else
        {
            Debug.LogError("nie ma punktu docelowego teleportacji");
        }
    }

}
