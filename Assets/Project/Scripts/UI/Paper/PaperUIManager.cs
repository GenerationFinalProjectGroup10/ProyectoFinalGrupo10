using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PaperUIManager : MonoBehaviour
{
    [Header("Panel del Prompt (Presiona E)")]
    public GameObject promptPanel;

    [Header("Panel de la Nota (Instrucciones)")]
    public GameObject notePanel;

    [Header("Animación (Opcional)")]
    [Tooltip("Velocidad del fade in/out")]
    public float fadeSpeed = 3f;

    private CanvasGroup promptCanvasGroup;
    private CanvasGroup noteCanvasGroup;

    void Awake()
    {
        // Obtener o agregar CanvasGroup para animaciones de fade
        if (promptPanel != null)
        {
            promptCanvasGroup = promptPanel.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
                promptCanvasGroup = promptPanel.AddComponent<CanvasGroup>();
        }

        if (notePanel != null)
        {
            noteCanvasGroup = notePanel.GetComponent<CanvasGroup>();
            if (noteCanvasGroup == null)
                noteCanvasGroup = notePanel.AddComponent<CanvasGroup>();
        }

        // Asegurar que todo esté oculto al inicio
        ShowPrompt(false);
        ShowNote(false);
    }

    public void ShowPrompt(bool show)
    {
        if (promptPanel == null) return;

        StopAllCoroutines();
        StartCoroutine(FadePanel(promptCanvasGroup, show));
    }

    public void ShowNote(bool show)
    {
        if (notePanel == null) return;

        if (show)
        {
            notePanel.SetActive(true);
            StartCoroutine(FadePanel(noteCanvasGroup, true));

            // Desbloquear cursor para poder hacer clic en "Cerrar"
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }
        else
        {
            StartCoroutine(FadeAndDeactivate(noteCanvasGroup, notePanel));

            // Volver a bloquear el cursor (ajusta según tu juego)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Fade suave de transparencia
    private IEnumerator FadePanel(CanvasGroup cg, bool show)
    {
        if (cg == null) yield break;

        float targetAlpha = show ? 1f : 0f;
        cg.gameObject.SetActive(true);

        while (!Mathf.Approximately(cg.alpha, targetAlpha))
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        cg.alpha = targetAlpha;
        if (!show) cg.gameObject.SetActive(false);
    }

    private IEnumerator FadeAndDeactivate(CanvasGroup cg, GameObject panel)
    {
        if (cg == null) yield break;

        float targetAlpha = 0f;
        while (!Mathf.Approximately(cg.alpha, targetAlpha))
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        cg.alpha = 0f;
        panel.SetActive(false);
    }
}