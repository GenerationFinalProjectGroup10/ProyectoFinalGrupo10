using UnityEngine;

public class RelojPuzzle : MonoBehaviour, IInteractuable
{
    public GameObject piezaVisual;
    public GameObject llave;
    public Animator animator;

    private bool estaCompleto = false;

    public void Interactuar()
    {
        if (!estaCompleto)
        {
            if (Inventario.Instance.TieneItem("PiezaReloj"))
            {
                CompletarReloj();
            }
            else
            {
                Debug.Log("Falta una pieza...");
            }
        }
    }

    void CompletarReloj()
    {
        estaCompleto = true;

        Inventario.Instance.RemoverItem("PiezaReloj");

        piezaVisual.SetActive(true);

        Debug.Log("Colocaste la pieza...");

        if (animator != null)
            animator.SetTrigger("Activar");

        Invoke("GenerarLlave", 2f);
    }

    void GenerarLlave()
    {
        llave.SetActive(true);
        Debug.Log("El reloj revela una llave 🔑");
    }
}