using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Vector2 input;
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Input clásico
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Animaciones
        animator.SetFloat("Horizontal", input.x);
        animator.SetFloat("Vertical", input.y);
        animator.SetBool("isMoving", input.sqrMagnitude > 0);
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(input.x, 0f, input.y);

        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }
}