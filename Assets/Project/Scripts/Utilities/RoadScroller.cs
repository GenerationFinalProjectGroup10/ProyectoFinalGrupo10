using UnityEngine;

public class RoadScroller : MonoBehaviour
{
    public float speed = 2f;

    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        offset.x += speed * Time.deltaTime;
        rend.material.mainTextureOffset = offset;
    }
}