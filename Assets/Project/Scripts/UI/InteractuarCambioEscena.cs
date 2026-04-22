using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractuarCambioEscena : MonoBehaviour
{
    public string nombreEscena;
    public KeyCode tecla = KeyCode.E;

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
            cerca = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            cerca = false;
    }
}