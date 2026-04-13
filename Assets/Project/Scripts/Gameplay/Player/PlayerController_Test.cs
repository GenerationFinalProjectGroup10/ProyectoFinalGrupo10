using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController_Test : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Vector2 moveInput;
    private CharacterController controller;
    private Animator animator;

    [SerializeField] private float speed = 5f;

    void Awake()
    {
        controls = new InputSystem_Actions();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();

        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        controller.Move(moveDirection * speed * Time.deltaTime);
    }
}