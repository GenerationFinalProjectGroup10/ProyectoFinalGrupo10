using UnityEngine;

public class InventoryCanvasPersistent : MonoBehaviour
{
    private static InventoryCanvasPersistent instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}