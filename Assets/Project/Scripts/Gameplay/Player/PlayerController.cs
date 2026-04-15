using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public float interactionDistance = 3f;
    [SerializeField] private UI_Inventory uiInventory; //Agregar al script del player

    private Inventory inventory; //Agregar al script del player
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private InputAction interactAction;

    void Start()
    {
        // Obtener acciones
        interactAction = playerInput.actions["Interact"]; // Acción "E"

        inventory = new Inventory(); //Agregar al script del player
        uiInventory.SetInventory(inventory); //Agregar al script del player

        /*ItemWorld.SpawnItemWorld(new Vector3(-4, 2, 2), new Item { itemType = Item.ItemType.Key, amount = 1 }); // Prueba de spawn
        ItemWorld.SpawnItemWorld(new Vector3(2, 2, 2), new Item { itemType = Item.ItemType.Coin, amount = 1 }); // Prueba de spawn*/
    }
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    //Agregar esta función al script del player
    private void OnTriggerEnter(Collider collider)
{
    ItemWorld itemWorld = collider.GetComponent<ItemWorld>();
    if (itemWorld != null)
    {
        // Obtener item del mundo
        Item pickedItem = itemWorld.GetItem();

        // Añadir una copia limpia
        inventory.AddItem(new Item {
            itemType = pickedItem.itemType,
            amount = 1
        });

        itemWorld.DestroySelf();
    }
}
    void Update()
    {
        // Leer input Movimiento
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        // Movimiento
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }

}