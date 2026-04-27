using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivacionOso : MonoBehaviour
{
    private bool estaCerca = false;

    void Update()
    {
        if (estaCerca && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("Cinematica_Oso");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) estaCerca = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) estaCerca = false;
    }
}