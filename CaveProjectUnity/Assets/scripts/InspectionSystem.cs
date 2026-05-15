using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class InspectionSystem : MonoBehaviour
{
    [Header("Referências")]
    public Transform inspectionPoint; // Arraste o objeto "InspectionPoint" (filho da câmera)
    public TextMeshProUGUI hotspotText; // Seu texto de descrição
    public GameObject reticle; // Sua mira da tela
    public LayerMask interactableLayer; // Layer "Interactable"

    [Header("Configurações")]
    public float rotationSpeed = 0.2f;
    public float centerDuration = 0.5f;
    public float interactDistance = 3f;

    private InspectableItem _currentItem;
    private InspectionHotspot _lastHovered;
    private bool _isInspecting;
    private bool _isCentering;

    // Referência ao script de movimento do Player para desativar
    private MonoBehaviour _playerMovement;
    private MouseLook _mouseLook;

    private Vector3 _debugRayOrigin;
private Vector3 _debugRayDirection;
private float _debugHitDistance;
private float _debugSphereRadius;
private bool _didHit;

    void Start()
    {
        // Tenta encontrar os scripts de movimento no próprio objeto ou no pai
        _mouseLook = GetComponentInChildren<MouseLook>();
        // Se o seu script de andar tiver outro nome, troque 'MonoBehaviour' pelo nome dele
        //_playerMovement = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        if (!_isInspecting)
        {
            HandleWorldRaycast();
        }
        else if (!_isCentering)
        {
            HandleInspectionInput();

            // Sair da inspeção
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                StopInspecting();
            }
        }
    }

    // 1. RAIO PARA INICIAR A INSPEÇÃO (No mundo)
    void HandleWorldRaycast()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                InspectableItem item = hit.collider.GetComponent<InspectableItem>();
                if (item != null) StartInspecting(item);
            }
        }
    }

    // 2. LÓGICA DENTRO DA INSPEÇÃO (Highlight, Clique e Rotação)
    void HandleInspectionInput()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            ResetLastHover();
            Vector2 delta = Mouse.current.delta.ReadValue();
            _currentItem.transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
            _currentItem.transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.World);

            Physics.SyncTransforms();

            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.SphereCast(ray, 0.05f, out hit, 10f, interactableLayer))
        {
            if (hit.collider.TryGetComponent<InspectionHotspot>(out var hotspot))
            {
                if (_lastHovered != hotspot)
                {
                    ResetLastHover();
                    _lastHovered = hotspot;
                    _lastHovered.SetHighlight(true);
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    StartCoroutine(CenterHotspotRoutine(hotspot));
                }
            }
            else { ResetLastHover(); }
        }
        else { ResetLastHover(); }
    }

    public void StartInspecting(InspectableItem item)
    {
        _isInspecting = true;
        _currentItem = item;

        // Salva estado original
        _currentItem.originalParent = item.transform.parent;
        _currentItem.originalPosition = item.transform.localPosition;
        _currentItem.originalRotation = item.transform.localRotation;

        // Configura física e posição
        _currentItem.gameObject.layer = LayerMask.NameToLayer("Default");
        _currentItem.transform.SetParent(inspectionPoint);
        _currentItem.transform.localPosition = Vector3.zero;

        // UI e Cursor
        Time.timeScale = 0;
        if (_mouseLook) _mouseLook.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        reticle.SetActive(false);
    }

    public void StopInspecting()
    {
        ResetLastHover();

        _currentItem.gameObject.layer = LayerMask.NameToLayer("Interactable");
        _currentItem.transform.SetParent(_currentItem.originalParent);
        _currentItem.transform.localPosition = _currentItem.originalPosition;
        _currentItem.transform.localRotation = _currentItem.originalRotation;

        Time.timeScale = 1;
        if (_mouseLook) _mouseLook.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        reticle.SetActive(true);
        hotspotText.text = "";

        _isInspecting = false;
        _currentItem = null;
    }

    IEnumerator CenterHotspotRoutine(InspectionHotspot hotspot)
    {
        _isCentering = true;
        hotspotText.text = hotspot.description;

        Transform targetFace = hotspot.faceDirection != null ? hotspot.faceDirection : hotspot.transform;
        float elapsed = 0;
        Quaternion startRot = _currentItem.transform.rotation;

        while (elapsed < centerDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / centerDuration);

            Vector3 dirToCam = Camera.main.transform.position - targetFace.position;
            if (dirToCam != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirToCam) * Quaternion.Inverse(targetFace.localRotation);
                _currentItem.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            }
            yield return null;
        }
        _isCentering = false;
    }

    void ResetLastHover()
    {
        if (_lastHovered != null)
        {
            _lastHovered.SetHighlight(false);
            _lastHovered = null;
        }
    }
}