using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInspection : MonoBehaviour
{
    [Header("Ustawienia inspekcji")]
    public float raycastDistance = 3f;
    public string inspectableTag = "Inspectable";
    public LayerMask inspectableLayer;
    public float rotationSpeed = 5f;
    public Vector3 inspectionOffset = new Vector3(0, 0, 2f);
    public float inspectionScale = 1.5f;

    [Header("UI")]
    public Text inspectionText;
    public GameObject inspectionUI;

    // Referencje do skryptów gracza
    private PlayerCam playerCam;
    private PlayerMovement playerMovement;
    private Camera mainCamera;

    // Zmienne do inspekcji
    private GameObject currentInspectable;
    private bool isInspecting = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    // Zapisane wartoœci do przywrócenia po inspekcji
    private float originalSensX;
    private float originalSensY;
    private CursorLockMode originalLockMode;
    private bool originalCursorVisible;

    public ItemDescriptionUIManager descriptionManager;


    void Start()
    {
        // Pobierz g³ówn¹ kamerê
        mainCamera = Camera.main;

        // ZnajdŸ i zapisz referencje do skryptów gracza
        playerCam = mainCamera.GetComponent<PlayerCam>();
        if (playerCam == null)
        {
            playerCam = FindObjectOfType<PlayerCam>();
        }

        // ZnajdŸ obiekt gracza ze skryptem PlayerMovement
        playerMovement = FindObjectOfType<PlayerMovement>();

        // SprawdŸ, czy wszystkie potrzebne komponenty zosta³y znalezione
        if (playerCam == null)
        {
            Debug.LogError("Nie znaleziono skryptu PlayerCam!");
        }

        if (playerMovement == null)
        {
            Debug.LogError("Nie znaleziono skryptu PlayerMovement!");
        }

        // Ukryj tekst interakcji na pocz¹tku
        if (inspectionText != null)
        {
            inspectionText.gameObject.SetActive(false);
        }

        // Ukryj UI inspekcji na pocz¹tku
        if (inspectionUI != null)
        {
            inspectionUI.SetActive(false);
        }

        // Upewnij siê, ¿e warstwa inspekcji jest ustawiona
        if (inspectableLayer.value == 0)
        {
            Debug.LogWarning("Warstwa inspekcji nie zosta³a ustawiona! Ustawiam na domyœln¹.");
            inspectableLayer = LayerMask.GetMask("Default");
        }
    }

    void Update()
    {
        if (isInspecting)
        {
            HandleInspectionControls();
        }
        else
        {
            CheckForInspectable();
        }
    }

    void CheckForInspectable()
    {
        // Jeœli ju¿ inspekcjonujemy, nie szukaj nowych obiektów
        if (isInspecting) return;

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        bool hitInspectable = false;

        // Debug ray - pomocny podczas testowania
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        if (Physics.Raycast(ray, out hit, raycastDistance, inspectableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            // SprawdŸ czy obiekt ma tag "Inspectable"
            if (hitObject.CompareTag(inspectableTag))
            {
                hitInspectable = true;

                // Poka¿ tekst interakcji
                if (inspectionText != null)
                {
                    inspectionText.text = "Naciœnij [E], aby obejrzeæ";
                    inspectionText.gameObject.SetActive(true);
                }

                // Rozpocznij inspekcjê, jeœli naciœniêto E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    BeginInspection(hitObject);
                }
            }
        }

        // Ukryj tekst, jeœli nie patrzymy na inspekcjonowalny obiekt
        if (!hitInspectable && inspectionText != null && inspectionText.gameObject.activeSelf)
        {
            inspectionText.gameObject.SetActive(false);
        }
    }

    void BeginInspection(GameObject inspectObject)
    {
        if (inspectObject == null) return;

        // Zapisz odniesienie do obiektu
        currentInspectable = inspectObject;

        // Zapisz oryginaln¹ transformacjê
        originalPosition = currentInspectable.transform.position;
        originalRotation = currentInspectable.transform.rotation;
        originalScale = currentInspectable.transform.localScale;

        // Zapisz stan kamery i kursora
        if (playerCam != null)
        {
            originalSensX = playerCam.sensX;
            originalSensY = playerCam.sensY;

            // Wy³¹cz kamerê gracza tymczasowo
            playerCam.enabled = false;
        }

        // Wy³¹cz ruch gracza
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Zapisz i zmieñ ustawienia kursora
        originalLockMode = Cursor.lockState;
        originalCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Przenieœ obiekt przed kamerê
        currentInspectable.transform.position = mainCamera.transform.position + mainCamera.transform.forward * inspectionOffset.z +
                                               mainCamera.transform.up * inspectionOffset.y +
                                               mainCamera.transform.right * inspectionOffset.x;

        // Obróæ obiekt, aby by³ skierowany do kamery
        currentInspectable.transform.rotation = Quaternion.LookRotation(
            mainCamera.transform.position - currentInspectable.transform.position);

        // Powiêksz obiekt dla lepszej widocznoœci
        currentInspectable.transform.localScale *= inspectionScale;

        // Wy³¹cz kolizje podczas inspekcji
        Collider[] colliders = currentInspectable.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Wy³¹cz Rigidbody, jeœli istnieje
        Rigidbody rb = currentInspectable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Poka¿ UI inspekcji
        if (inspectionUI != null)
        {
            inspectionUI.SetActive(true);
        }

        // Ukryj tekst interakcji
        if (inspectionText != null)
        {
            inspectionText.gameObject.SetActive(false);
        }

        isInspecting = true;

        Debug.Log("Rozpoczêto inspekcjê obiektu: " + currentInspectable.name);

        ItemDescription itemDesc = inspectObject.GetComponent<ItemDescription>();

        if (itemDesc != null && descriptionManager != null)
        {
            descriptionManager.ShowDescription(itemDesc.itemName, itemDesc.description);
        }
    }

    void HandleInspectionControls()
    {
        if (currentInspectable == null)
        {
            EndInspection();
            return;
        }

        // Obracanie obiektu za pomoc¹ myszy
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxisRaw("Mouse Y") * rotationSpeed;

            currentInspectable.transform.Rotate(Vector3.up, -mouseX, Space.World);
            currentInspectable.transform.Rotate(Vector3.right, mouseY, Space.World);
        }

        // Wyjœcie z trybu inspekcji
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            EndInspection();
        }
    }

    void EndInspection()
    {
        if (currentInspectable == null) return;

        // Przywróæ oryginaln¹ transformacjê
        currentInspectable.transform.position = originalPosition;
        currentInspectable.transform.rotation = originalRotation;
        currentInspectable.transform.localScale = originalScale;

        // W³¹cz kolizje
        Collider[] colliders = currentInspectable.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        // W³¹cz Rigidbody, jeœli istnieje
        Rigidbody rb = currentInspectable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // W³¹cz z powrotem kamerê gracza i przywróæ czu³oœæ
        if (playerCam != null)
        {
            playerCam.sensX = originalSensX;
            playerCam.sensY = originalSensY;
            playerCam.enabled = true;
        }

        // W³¹cz ruch gracza
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Przywróæ stan kursora
        Cursor.lockState = originalLockMode;
        Cursor.visible = originalCursorVisible;

        // Ukryj UI inspekcji
        if (inspectionUI != null)
        {
            inspectionUI.SetActive(false);
        }

        Debug.Log("Zakoñczono inspekcjê obiektu");
        currentInspectable = null;
        isInspecting = false;

        if (descriptionManager != null)
        {
            descriptionManager.HideDescription();
        }
    }
}