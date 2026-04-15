using UnityEngine;

public class ItemWorldSpawner : MonoBehaviour
{

    public Item item; // Asignar en el inspector el tipo de item a spawnear

    private void Start()
    {
        ItemWorld.SpawnItemWorld(transform.position, item);
        Destroy(gameObject);
    }

}
