using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public float interactionDistance = 3f; //Agregar al script del player
    [SerializeField] private UI_Inventory uiInventory; //Agregar al script del player

    private Inventory inventory; //Agregar al script del player
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private InputAction interactAction;

    // 🔥 Lista de interactuables dentro del trigger
    private readonly System.Collections.Generic.List<IInteractable> interactables = new System.Collections.Generic.List<IInteractable>();
    private IInteractable currentInteractable;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        // Obtener acciones
        interactAction = playerInput.actions["Interact"]; // Acción "E"

        // Usar inventario global único
        inventory = InventoryManager.Instance.inventory;

        if (uiInventory != null)
        {
            uiInventory.SetInventory(inventory);
        }
        else
        {
            Debug.LogWarning("UI_Inventory no está asignado en PlayerController.");
        }
    }

    void Update()
    {
        // Movimiento
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        // 🔥 INTERACCIÓN CON E
        if (interactAction.triggered)
        {
            //Agregar al script del player
            if (currentInteractable != null)
            {
                currentInteractable.Interact(this);
            }
        }
    }

    // 🔥 Detecta entrada a zona de interacción
    private void OnTriggerEnter(Collider other)
    {
        if (TryGetInteractable(other, out IInteractable interactable))
        {
            if (!interactables.Contains(interactable))
            {
                interactables.Add(interactable);
            }
            currentInteractable = interactable;
            Debug.Log("Interactuable detectado: " + other.name);
        }

        // Agregar al script del player
        ItemWorld itemWorld = other.GetComponent<ItemWorld>();
        if (itemWorld == null)
        {
            itemWorld = other.GetComponentInParent<ItemWorld>();
        }

        if (itemWorld != null)
        {
            Item pickedItem = itemWorld.GetItem();

            if (pickedItem != null)
            {
                inventory.AddItem(new Item
                {
                    itemType = pickedItem.itemType,
                    amount = 1
                });
            }

            itemWorld.DestroySelf();
        }
    }

    // 🔥 Salir de zona de interacción
    private void OnTriggerExit(Collider other)
    {
        if (TryGetInteractable(other, out IInteractable interactable))
        {
            interactables.Remove(interactable);
            if (interactables.Count > 0)
            {
                currentInteractable = interactables[interactables.Count - 1];
            }
            else
            {
                currentInteractable = null;
            }

            Debug.Log("Saliste del interactuable: " + other.name);
        }
    }

    private bool TryGetInteractable(Collider other, out IInteractable interactable)
    {
        interactable = other.GetComponent(typeof(IInteractable)) as IInteractable;
        if (interactable != null)
        {
            return true;
        }

        interactable = other.GetComponentInParent(typeof(IInteractable)) as IInteractable;
        return interactable != null;
    }

    public Inventory GetInventory()
    {
        return inventory;
    }
}