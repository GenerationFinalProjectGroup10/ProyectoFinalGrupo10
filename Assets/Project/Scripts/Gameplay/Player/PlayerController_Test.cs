using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_Test : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Vector2 moveInput;
    private Animator animator;

    [SerializeField] private float speed = 5f;

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
        
        animator.SetBool("IsMoving", moveInput.sqrMagnitude > 0);

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }
}