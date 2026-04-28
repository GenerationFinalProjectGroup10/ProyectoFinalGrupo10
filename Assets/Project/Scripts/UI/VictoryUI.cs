using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class VictoryUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("El panel raíz del mensaje de victoria (empieza desactivado).")]
    public GameObject panelVictoria;

    [Tooltip("Texto principal, ej: '¡Hemos Sobrevivido!'")]
    public TextMeshProUGUI textoTitulo;

    [Tooltip("Texto secundario opcional, ej: tiempo restante.")]
    public TextMeshProUGUI textoDetalle;

    [Tooltip("Botón para ir a los créditos.")]
    public Button botonCreditos;  // ✅ nombre corregido

    [Header("Animación")]
    [Tooltip("Duración del fade-in del panel en segundos.")]
    public float fadeInDuration = 1.2f;

    private CanvasGroup _canvasGroup;

    void Awake()
    {
        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (botonCreditos != null)
            botonCreditos.onClick.AddListener(IrACreditos);  // ✅ nombre corregido
    }

    public void Mostrar()
    {
        if (panelVictoria == null) return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        if (textoDetalle != null && ClockManager.Instance != null)
        {
            var (h, m, s) = ClockManager.Instance.GetTimerTime();
            textoDetalle.text = $"Tiempo restante: {h:00}:{m:00}:{s:00}";
        }

        panelVictoria.SetActive(true);

        _canvasGroup = panelVictoria.GetComponent<CanvasGroup>();
        if (_canvasGroup != null)
            StartCoroutine(FadeIn());

        Time.timeScale = 0f;
        Debug.Log("🏆 ¡Victoria! El jugador regresó a Mundo1.");
    }

    private IEnumerator FadeIn()
    {
        _canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }

    public void IrACreditos()  // ✅ nombre corregido
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None; // 🆕
        Cursor.visible = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }
}