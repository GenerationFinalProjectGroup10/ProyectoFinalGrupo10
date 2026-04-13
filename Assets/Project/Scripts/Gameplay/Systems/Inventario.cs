using System.Collections.Generic;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    public static Inventario Instance;

    private List<string> items = new List<string>();

    void Awake()
    {
        Instance = this;
    }

    public void AgregarItem(string item)
    {
        items.Add(item);
        Debug.Log("Recogiste: " + item);
    }

    public bool TieneItem(string item)
    {
        return items.Contains(item);
    }

    public void RemoverItem(string item)
    {
        items.Remove(item);
    }
}