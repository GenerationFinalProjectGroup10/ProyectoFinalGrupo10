using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;

    [Header("Camera Settings")]
    public Transform cameraPivot;
    public float mouseSensitivity = 3f;
    public float rotationSmooth = 10f;
    public float minPitch = -15f;
    public float maxPitch = 35f;

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

    private float yaw;
    private float pitch;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        TryAutoAssignReferences();
    }

    void Start()
    {
        TryAutoAssignReferences();

        yaw = transform.eulerAngles.y;
        pitch = 0f;

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (rb == null)
        {
            Debug.LogError("No Rigidbody en Player");
            enabled = false;
            return;
        }

        if (animator == null)
            Debug.LogError("No Animator en Player");
    }

    void OnEnable()
    {
        TryAutoAssignReferences();
    }

    void Update()
    {
        TryAutoAssignReferences();

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        HandleCameraRotation();

        lookDirection = transform.forward;

        if (animator != null)
        {
            if (input.sqrMagnitude > 0)
            {
                animator.SetFloat("Horizontal", input.x);
                animator.SetFloat("Vertical", input.y);
            }
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

        Quaternion moveRotation = Quaternion.Euler(0f, yaw, 0f);

        Vector3 forward = moveRotation * Vector3.forward;
        Vector3 right = moveRotation * Vector3.right;

        Vector3 moveDir =
            forward * input.y +
            right * input.x;

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        rb.MovePosition(
            rb.position +
            moveDir * speed * Time.fixedDeltaTime
        );
    }

    void HandleCameraRotation()
    {
        if (cameraPivot == null) return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        Quaternion targetPitch = Quaternion.Euler(pitch, 0f, 0f);

        cameraPivot.localRotation = Quaternion.Lerp(
            cameraPivot.localRotation,
            targetPitch,
            rotationSmooth * Time.deltaTime
        );
    }

    void HandleRaycast()
    {
        if (rayOrigin == null) return;
        if (InventoryManager.Instance == null) return;

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

        var inventory = InventoryManager.Instance != null ? InventoryManager.Instance.inventory : null;
        if (inventory != null)
        {
            inventory.RemoveItem(framePart1, 1);
            inventory.RemoveItem(framePart2, 1);
            inventory.RemoveItem(framePart3, 1);
            inventory.RemoveItem(framePart4, 1);
            inventory.AddItem(clockPart1, 1);
        }

        UI_Message.Instance?.ShowTemporary(
            "¡Completaste el portaretrato! Obtienes la pieza del reloj.",
            frameRewardMessageDuration
        );
        yield break;
    }

    private void TryAutoAssignReferences()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();

        if (rayOrigin == null)
        {
            if (cameraPivot != null)
                rayOrigin = cameraPivot;
            else if (Camera.main != null)
                rayOrigin = Camera.main.transform;
        }

        if (cameraPivot == null && Camera.main != null)
            cameraPivot = Camera.main.transform;
    }
}