using UnityEngine;
using System.Collections;

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
    [SerializeField] private ItemSO keyItem;

    [Header("Message Durations")]
    [SerializeField] private float clockRewardMessageDuration = 4f;
    [SerializeField] private float frameRewardHintDelay = 1.8f;
    [SerializeField] private float frameRewardMessageDuration = 5f;

    private bool frameRewardGiven;
    private bool clockRewardGiven;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
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
        HandleCombinationHints();

        if (Input.GetKeyDown(KeyCode.E)) TryInteract();
        if (Input.GetKeyDown(KeyCode.C)) TryCombine();
    }

    void FixedUpdate()
    {
        if (rb == null) return;
        Vector3 movement = new Vector3(input.x, 0f, input.y);
        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }

    void HandleRaycast()
    {
        if (rayOrigin == null || InventoryManager.Instance == null) return;

        Debug.DrawRay(rayOrigin.position, lookDirection * rayDistance, Color.red);

        if (Physics.Raycast(rayOrigin.position, lookDirection, out RaycastHit hit, rayDistance, interactableLayer))
        {
            var interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;
                    var message = interactable.GetInteractMessage();
                    if (!string.IsNullOrWhiteSpace(message))
                        UI_Message.Instance?.ShowInteraction(message);
                }

                lastHit = hit;
                return;
            }
        }

        currentInteractable = null;
        lastHit = default;
        UI_Message.Instance?.HideInteraction();
    }

    void HandleCombinationHints()
    {
        if (InventoryManager.Instance == null) return;
        var inventory = InventoryManager.Instance.inventory;
        if (inventory == null) return;

        bool hasAllFrameParts =
            framePart1 != null && framePart2 != null && framePart3 != null && framePart4 != null &&
            inventory.HasItem(framePart1, 1) &&
            inventory.HasItem(framePart2, 1) &&
            inventory.HasItem(framePart3, 1) &&
            inventory.HasItem(framePart4, 1);

        bool hasClockPart1 = clockPart1 != null && inventory.HasItem(clockPart1, 1);
        bool isLookingAtSecondClockPiece = false;
        bool isInClockProximity = false;

        if (lastHit.collider != null)
        {
            var openObject = lastHit.collider.GetComponentInParent<OpenObject>();
            isLookingAtSecondClockPiece = openObject != null && openObject.IsSecondClockPiece();

            var clockMessage = lastHit.collider.GetComponentInParent<ClockProximityMessage>();
            isInClockProximity = clockMessage != null;
        }

        if (!frameRewardGiven && hasAllFrameParts)
        {
            UI_Message.Instance?.ShowInteraction("Presiona C para juntar las piezas del retrato");
        }
        else if (!clockRewardGiven && hasClockPart1 && isLookingAtSecondClockPiece && !isInClockProximity)
        {
            UI_Message.Instance?.ShowInteraction("Presiona C para juntar la pieza del reloj con la encontrada");
        }
    }

    void TryInteract()
    {
        if (lastHit.collider == null) return;

        var interactable = lastHit.collider.GetComponentInParent<IInteractable>();
        if (interactable != null)
            interactable.Interact(this);
    }

    void TryCombine()
    {
        if (InventoryManager.Instance == null) return;

        var inventory = InventoryManager.Instance.inventory;
        if (inventory == null) return;

        bool hasClockPart1 = clockPart1 != null && inventory.HasItem(clockPart1, 1);
        var openObject = lastHit.collider != null ? lastHit.collider.GetComponentInParent<OpenObject>() : null;
        bool isSecondClockPiece = openObject != null && openObject.IsSecondClockPiece();

        if (hasClockPart1 && isSecondClockPiece && openObject != null && !clockRewardGiven)
        {
            inventory.RemoveItem(clockPart1, 1);
            inventory.AddItem(keyItem, 1);
            openObject.ConsumeWorldObject();
            clockRewardGiven = true;

            UI_Message.Instance?.ShowTemporary(
                "¡Uniste la pieza del reloj con la encontrada y obtuviste la llave!",
                clockRewardMessageDuration
            );
            return;
        }

        bool hasAllFrameParts =
            framePart1 != null && framePart2 != null && framePart3 != null && framePart4 != null &&
            inventory.HasItem(framePart1, 1) &&
            inventory.HasItem(framePart2, 1) &&
            inventory.HasItem(framePart3, 1) &&
            inventory.HasItem(framePart4, 1);

        if (!frameRewardGiven && hasAllFrameParts)
            StartCoroutine(CompleteFrameReward());
    }

    private IEnumerator CompleteFrameReward()
    {
        frameRewardGiven = true;

        var inventory = InventoryManager.Instance.inventory;
        if (inventory != null)
        {
            inventory.RemoveItem(framePart1, 1);
            inventory.RemoveItem(framePart2, 1);
            inventory.RemoveItem(framePart3, 1);
            inventory.RemoveItem(framePart4, 1);
            inventory.AddItem(clockPart1, 1);
        }

        UI_Message.Instance?.ShowTemporary("¡Completaste el portaretrato! Obtienes la pieza del reloj.", frameRewardMessageDuration);
        yield break;
    }
}