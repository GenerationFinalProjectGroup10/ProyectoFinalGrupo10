using UnityEngine;
using UnityEngine.UI; // Necesario para controlar la imagen
using UnityEngine.SceneManagement;
using System.Collections; // Necesario para las Corrutinas

public class FinalizadorCinematica : MonoBehaviour
{
    public Image fadeImage; 
    public float velocidadFade = 0.5f; 
    public float tiempoEnNegro = 2f; 

    public void IrAlMundo2()
    {
        StartCoroutine(SecuenciaFinal());
    }

    IEnumerator SecuenciaFinal()
    {
        float alfa = 0;


        while (alfa < 1)
        {
            alfa += Time.deltaTime * velocidadFade;
            fadeImage.color = new Color(0, 0, 0, alfa);
            yield return null;
        }

        yield return new WaitForSeconds(tiempoEnNegro);

        SceneManager.LoadScene("Mundo2");
    }
}