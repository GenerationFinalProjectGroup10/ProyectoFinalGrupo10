using UnityEngine;

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

    [SerializeField] private ItemSO clueItem;
    [SerializeField] private ItemSO keyItem;

    private float yaw;
    private float pitch;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // 🔥 INICIAR MIRANDO AL FRENTE
        yaw = 0f;
        pitch = 0f;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (rb == null)
        {
            Debug.LogError("No Rigidbody en Player");
            return;
        }

        if (animator == null)
        {
            Debug.LogError("No Animator en Player");
        }
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        HandleCameraRotation();

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
        HandleClueCombination();

        if (Input.GetKeyDown(KeyCode.E))
            TryInteract();
    }

    void FixedUpdate()
    {
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