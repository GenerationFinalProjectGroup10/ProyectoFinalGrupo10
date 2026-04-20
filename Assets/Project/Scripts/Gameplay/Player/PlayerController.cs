using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;

    [Header("Raycast Settings")]
    public Transform rayOrigin;
    public float rayDistance = 3f;
    public LayerMask interactableLayer;

    private PlayerInput playerInput;
    private Vector2 moveInput;
    private InputAction interactAction;
    private RaycastHit lastHit;

    [SerializeField] private ItemSO clueItem;
    [SerializeField] private ItemSO keyItem;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerController: No hay un PlayerInput en el Player");
            return;
        }

        interactAction = playerInput.actions["Interact"];
    }

    void Update()
    {
        HandleRaycast();
        HandleClueCombination();

        if (interactAction != null && interactAction.WasPressedThisFrame())
            TryInteract();

        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);

        if (movement != Vector3.zero)
            transform.forward = movement;

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }

    void HandleRaycast()
    {
        if (rayOrigin == null || InventoryManager.Instance == null)
            return;

        Debug.DrawRay(rayOrigin.position, rayOrigin.forward * rayDistance, Color.red);

        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, rayDistance, interactableLayer))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                UI_Message.Instance?.Show(interactable.GetInteractMessage(), persistent: true);
                lastHit = hit;
                return;
            }
        }

        lastHit = default;
        UI_Message.Instance?.Hide();
    }

    void TryInteract()
    {
        if (lastHit.collider == null) return;

        if (lastHit.collider.TryGetComponent<IInteractable>(out var interactable))
            interactable.Interact(this);
    }

    void HandleClueCombination()
    {
        if (InventoryManager.Instance == null || clueItem == null || keyItem == null)
            return;

        if (!InventoryManager.Instance.inventory.HasItem(clueItem, 2))
            return;

        UI_Message.Instance?.Show("Presiona C para combinar pistas", persistent: true);

        if (Input.GetKeyDown(KeyCode.C))
        {
            InventoryManager.Instance.inventory.RemoveItem(clueItem, 2);
            InventoryManager.Instance.inventory.AddItem(keyItem, 1);
            UI_Message.Instance?.Show("¡Pistas combinadas! nueva llave obtenida");
        }
    }
}