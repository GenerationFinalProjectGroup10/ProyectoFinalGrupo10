using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; 

public class FinalizadorCinematica : MonoBehaviour
{
    [Header("Configuración Visual")]
    public Image fadeImage; 
    public float velocidadFade = 0.5f; 
    public float tiempoEnNegro = 2f;

    [Header("Mensaje Final (Opcional)")]
    public GameObject objetoTexto; 
    public float tiempoConTexto = 3f;

    public void IrAlMundo2()
    {
        StartCoroutine(SecuenciaFinal("Mundo2", false));
    }

    public void IrAlFinalMalo()
    {
        StartCoroutine(SecuenciaFinal("MainMenu", true));
    }

    IEnumerator SecuenciaFinal(string nombreEscena, bool mostrarTexto)
    {
        float alfa = 0;

        while (alfa < 1)
        {
            alfa += Time.deltaTime * velocidadFade;
            fadeImage.color = new Color(0, 0, 0, alfa);
            yield return null;
        }

        if (mostrarTexto && objetoTexto != null)
        {
            objetoTexto.SetActive(true);
            yield return new WaitForSeconds(tiempoConTexto);
        }
        else
        {
            yield return new WaitForSeconds(tiempoEnNegro);
        }

        SceneManager.LoadScene(nombreEscena);
    }
}