using UnityEngine;

public class GrassLooper : MonoBehaviour
{
    public float speed = 5f;

    public float despawnX = -150f; // donde desaparece
    public float respawnX = 150f;  // donde reaparece

    void Update()
    {
        // mover hacia la izquierda
        transform.position += Vector3.left * speed * Time.deltaTime;

        // si se va muy a la izquierda
        if (transform.position.x < despawnX)
        {
            Vector3 pos = transform.position;
            pos.x = respawnX;
            transform.position = pos;
        }
    }
}