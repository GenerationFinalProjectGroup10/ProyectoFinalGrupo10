using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Inventory inventory;

    void Awake()
    {
        Instance = this;
        inventory = new Inventory();
    }
}