using UnityEngine;

public class Llave : MonoBehaviour, IInteractuable
{
    public void Interactuar()
    {
        Inventario.Instance.AgregarItem("LlaveSotano");
        Destroy(gameObject);
    }
}