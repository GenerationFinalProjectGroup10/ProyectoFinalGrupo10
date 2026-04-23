using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractuarCambioEscena : MonoBehaviour
{
    public string nombreEscena;
    public KeyCode tecla = KeyCode.E;
    public GameObject canvasInteraccion; // ← línea nueva

    private bool cerca = false;

    void Update()
    {
        if (cerca && Input.GetKeyDown(tecla))
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cerca = true;
            if (canvasInteraccion != null)
                canvasInteraccion.SetActive(true); // ← línea nueva
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cerca = false;
            if (canvasInteraccion != null)
                canvasInteraccion.SetActive(false); // ← línea nueva
        }
    }
}