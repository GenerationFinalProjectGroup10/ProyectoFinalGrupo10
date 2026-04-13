using UnityEngine;

public class PiezaReloj : MonoBehaviour, IInteractuable
{
    public void Interactuar()
    {
        Inventario.Instance.AgregarItem("PiezaReloj");
        Destroy(gameObject);
    }
}