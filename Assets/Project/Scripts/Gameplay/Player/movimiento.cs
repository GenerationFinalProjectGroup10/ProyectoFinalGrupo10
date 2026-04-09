using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Vector2 input;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(input.x, 0f, input.y);

        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }
}