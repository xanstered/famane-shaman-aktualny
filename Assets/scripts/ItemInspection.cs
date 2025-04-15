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

    // Referencje do skrypt�w gracza
    private PlayerCam playerCam;
    private PlayerMovement playerMovement;
    private Camera mainCamera;

    // Zmienne do inspekcji
    private GameObject currentInspectable;
    private bool isInspecting = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    // Zapisane warto�ci do przywr�cenia po inspekcji
    private float originalSensX;
    private float originalSensY;
    private CursorLockMode originalLockMode;
    private bool originalCursorVisible;

    public ItemDescriptionUIManager descriptionManager;


    void Start()
    {
        // Pobierz g��wn� kamer�
        mainCamera = Camera.main;

        // Znajd� i zapisz referencje do skrypt�w gracza
        playerCam = mainCamera.GetComponent<PlayerCam>();
        if (playerCam == null)
        {
            playerCam = FindObjectOfType<PlayerCam>();
        }

        // Znajd� obiekt gracza ze skryptem PlayerMovement
        playerMovement = FindObjectOfType<PlayerMovement>();

        // Sprawd�, czy wszystkie potrzebne komponenty zosta�y znalezione
        if (playerCam == null)
        {
            Debug.LogError("Nie znaleziono skryptu PlayerCam!");
        }

        if (playerMovement == null)
        {
            Debug.LogError("Nie znaleziono skryptu PlayerMovement!");
        }

        // Ukryj tekst interakcji na pocz�tku
        if (inspectionText != null)
        {
            inspectionText.gameObject.SetActive(false);
        }

        // Ukryj UI inspekcji na pocz�tku
        if (inspectionUI != null)
        {
            inspectionUI.SetActive(false);
        }

        // Upewnij si�, �e warstwa inspekcji jest ustawiona
        if (inspectableLayer.value == 0)
        {
            Debug.LogWarning("Warstwa inspekcji nie zosta�a ustawiona! Ustawiam na domy�ln�.");
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
        // Je�li ju� inspekcjonujemy, nie szukaj nowych obiekt�w
        if (isInspecting) return;

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        bool hitInspectable = false;

        // Debug ray - pomocny podczas testowania
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        if (Physics.Raycast(ray, out hit, raycastDistance, inspectableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Sprawd� czy obiekt ma tag "Inspectable"
            if (hitObject.CompareTag(inspectableTag))
            {
                hitInspectable = true;

                // Poka� tekst interakcji
                if (inspectionText != null)
                {
                    inspectionText.text = "Naci�nij [E], aby obejrze�";
                    inspectionText.gameObject.SetActive(true);
                }

                // Rozpocznij inspekcj�, je�li naci�ni�to E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    BeginInspection(hitObject);
                }
            }
        }

        // Ukryj tekst, je�li nie patrzymy na inspekcjonowalny obiekt
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

        // Zapisz oryginaln� transformacj�
        originalPosition = currentInspectable.transform.position;
        originalRotation = currentInspectable.transform.rotation;
        originalScale = currentInspectable.transform.localScale;

        // Zapisz stan kamery i kursora
        if (playerCam != null)
        {
            originalSensX = playerCam.sensX;
            originalSensY = playerCam.sensY;

            // Wy��cz kamer� gracza tymczasowo
            playerCam.enabled = false;
        }

        // Wy��cz ruch gracza
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Zapisz i zmie� ustawienia kursora
        originalLockMode = Cursor.lockState;
        originalCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Przenie� obiekt przed kamer�
        currentInspectable.transform.position = mainCamera.transform.position + mainCamera.transform.forward * inspectionOffset.z +
                                               mainCamera.transform.up * inspectionOffset.y +
                                               mainCamera.transform.right * inspectionOffset.x;

        // Obr�� obiekt, aby by� skierowany do kamery
        currentInspectable.transform.rotation = Quaternion.LookRotation(
            mainCamera.transform.position - currentInspectable.transform.position);

        // Powi�ksz obiekt dla lepszej widoczno�ci
        currentInspectable.transform.localScale *= inspectionScale;

        // Wy��cz kolizje podczas inspekcji
        Collider[] colliders = currentInspectable.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Wy��cz Rigidbody, je�li istnieje
        Rigidbody rb = currentInspectable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Poka� UI inspekcji
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

        Debug.Log("Rozpocz�to inspekcj� obiektu: " + currentInspectable.name);

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

        // Obracanie obiektu za pomoc� myszy
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxisRaw("Mouse Y") * rotationSpeed;

            currentInspectable.transform.Rotate(Vector3.up, -mouseX, Space.World);
            currentInspectable.transform.Rotate(Vector3.right, mouseY, Space.World);
        }

        // Wyj�cie z trybu inspekcji
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            EndInspection();
        }
    }

    void EndInspection()
    {
        if (currentInspectable == null) return;

        // Przywr�� oryginaln� transformacj�
        currentInspectable.transform.position = originalPosition;
        currentInspectable.transform.rotation = originalRotation;
        currentInspectable.transform.localScale = originalScale;

        // W��cz kolizje
        Collider[] colliders = currentInspectable.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        // W��cz Rigidbody, je�li istnieje
        Rigidbody rb = currentInspectable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // W��cz z powrotem kamer� gracza i przywr�� czu�o��
        if (playerCam != null)
        {
            playerCam.sensX = originalSensX;
            playerCam.sensY = originalSensY;
            playerCam.enabled = true;
        }

        // W��cz ruch gracza
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Przywr�� stan kursora
        Cursor.lockState = originalLockMode;
        Cursor.visible = originalCursorVisible;

        // Ukryj UI inspekcji
        if (inspectionUI != null)
        {
            inspectionUI.SetActive(false);
        }

        Debug.Log("Zako�czono inspekcj� obiektu");
        currentInspectable = null;
        isInspecting = false;

        if (descriptionManager != null)
        {
            descriptionManager.HideDescription();
        }
    }
}