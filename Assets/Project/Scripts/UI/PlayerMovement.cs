using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float interactionDistance = 3f;

    private PlayerInput playerInput;
    private Vector2 moveInput;
    private InputAction interactAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        // Obtener acciones
        interactAction = playerInput.actions["Interact"]; // Acción "E"
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