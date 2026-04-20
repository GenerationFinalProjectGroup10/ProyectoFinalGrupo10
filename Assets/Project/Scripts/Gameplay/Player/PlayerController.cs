using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;

    [Header("Raycast Settings")]
    public Transform rayOrigin;
    public float rayDistance = 3f;
    public LayerMask interactableLayer;

    private Vector2 input;
    private Rigidbody rb;
    private Animator animator;
    private RaycastHit lastHit;

    [SerializeField] private ItemSO clueItem;
    [SerializeField] private ItemSO keyItem;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (rb == null)
        {
            Debug.LogError("PlayerController: No hay un Rigidbody en el Player");
            return;
        }

        if (animator == null)
        {
            Debug.LogError("PlayerController: No hay un Animator en el Player");
        }
    }

    void Update()
    {
        // Input clásico
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Animaciones 
        if (animator != null)
        {
            animator.SetFloat("Horizontal", input.x);
            animator.SetFloat("Vertical", input.y);
            animator.SetBool("isMoving", input.sqrMagnitude > 0);
        }

        // Lógica original del segundo script
        HandleRaycast();
        HandleClueCombination();

        if (Input.GetKeyDown(KeyCode.E))  // Cambiado a E para Interact (Input.GetKeyDown en lugar de InputSystem)
            TryInteract();
    }

    void FixedUpdate()
    {
        // Movimiento con Rigidbody del primer script
        Vector3 movement = new Vector3(input.x, 0f, input.y);
        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
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
            UI_Message.Instance?.Show("¡Pistas combinadas! Nueva llave obtenida.");
        }
    }
}