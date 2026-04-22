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
    private Vector3 lookDirection = Vector3.forward;
    private IInteractable currentInteractable;

    [Header("Rewards")]
    [SerializeField] private ItemSO framePart1;
    [SerializeField] private ItemSO framePart2;
    [SerializeField] private ItemSO framePart3;
    [SerializeField] private ItemSO framePart4;
    [SerializeField] private ItemSO clockPart1;
    [SerializeField] private ItemSO clockPart2;
    [SerializeField] private ItemSO keyItem;

    [Header("Message Durations")]
    [SerializeField] private float interactMessageDuration = 2f;
    [SerializeField] private float frameRewardMessageDuration = 4f;
    [SerializeField] private float clockRewardMessageDuration = 4f;

    private bool frameRewardGiven;
    private bool clockRewardGiven;

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
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(input.x, 0f, input.y);

        if (movement != Vector3.zero)
            lookDirection = movement.normalized;

        if (animator != null)
        {
            animator.SetFloat("Horizontal", input.x);
            animator.SetFloat("Vertical", input.y);
            animator.SetBool("isMoving", input.sqrMagnitude > 0);
        }

        HandleRaycast();

        if (Input.GetKeyDown(KeyCode.E))
            TryInteract();

        HandleRewards();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 movement = new Vector3(input.x, 0f, input.y);
        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }

    void HandleRaycast()
    {
        if (rayOrigin == null || InventoryManager.Instance == null)
            return;

        Debug.DrawRay(rayOrigin.position, lookDirection * rayDistance, Color.red);

        if (Physics.Raycast(rayOrigin.position, lookDirection, out RaycastHit hit, rayDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;
                    string message = interactable.GetInteractMessage();

                    if (!string.IsNullOrWhiteSpace(message))
                        UI_Message.Instance?.Show(message, true);
                }

                lastHit = hit;
                return;
            }
        }

        currentInteractable = null;
        lastHit = default;
    }

    void TryInteract()
    {
        if (lastHit.collider == null) return;

        if (lastHit.collider.TryGetComponent<IInteractable>(out var interactable))
            interactable.Interact(this);
    }

    void HandleRewards()
    {
        if (InventoryManager.Instance == null) return;

        var inventory = InventoryManager.Instance.inventory;

        if (!frameRewardGiven &&
            framePart1 != null && framePart2 != null && framePart3 != null && framePart4 != null && clockPart1 != null)
        {
            bool hasAllFrameParts =
                inventory.HasItem(framePart1, 1) &&
                inventory.HasItem(framePart2, 1) &&
                inventory.HasItem(framePart3, 1) &&
                inventory.HasItem(framePart4, 1);

            if (hasAllFrameParts)
            {
                inventory.RemoveItem(framePart1, 1);
                inventory.RemoveItem(framePart2, 1);
                inventory.RemoveItem(framePart3, 1);
                inventory.RemoveItem(framePart4, 1);

                inventory.AddItem(clockPart1, 1);
                frameRewardGiven = true;

                UI_Message.Instance?.Show("¡Completaste el portaretrato! Obtienes la primera pieza del reloj.", false, frameRewardMessageDuration);
            }
        }

        if (!clockRewardGiven &&
            clockPart1 != null && clockPart2 != null && keyItem != null)
        {
            bool hasClockParts =
                inventory.HasItem(clockPart1, 1) &&
                inventory.HasItem(clockPart2, 1);

            if (hasClockParts)
            {
                inventory.RemoveItem(clockPart1, 1);
                inventory.RemoveItem(clockPart2, 1);

                inventory.AddItem(keyItem, 1);
                clockRewardGiven = true;

                UI_Message.Instance?.Show("¡Uniste las dos piezas del reloj y obtuviste la llave.", false, clockRewardMessageDuration);
            }
        }
    }
}