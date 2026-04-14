using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerPato : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Vector2 moveInput;
    private Animator animator;

    [SerializeField] private float speed = 5f;

    // 🔥 NUEVO
    [SerializeField] private float rangoInteraccion = 2f;
    private IInteractuable objetoCercano;

    void Awake()
    {
        controls = new InputSystem_Actions();
        animator = GetComponent<Animator>();
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();

        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
        animator.SetBool("isMoving", moveInput.sqrMagnitude > 0);

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        DetectarInteractuable();

        // 🔥 TECLA E
        if (Keyboard.current.eKey.wasPressedThisFrame && objetoCercano != null)
        {
            objetoCercano.Interactuar();
        }
    }

    void DetectarInteractuable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, rangoInteraccion);

        objetoCercano = null;

        foreach (var hit in hits)
        {
            IInteractuable interactuable = hit.GetComponent<IInteractuable>();
            if (interactuable != null)
            {
                objetoCercano = interactuable;
                break;
            }
        }
    }
}