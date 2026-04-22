using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class UI_Message : MonoBehaviour
{
    public static UI_Message Instance;

    [Header("UI References")]
    [SerializeField] private RectTransform messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Settings")]
    [SerializeField] private float defaultDisplayDuration = 2f;
    [SerializeField] private Vector2 padding = new Vector2(30f, 18f);

    private Coroutine hideCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (messagePanel == null)
        {
            Debug.LogError("UI_Message: messagePanel no asignado");
            return;
        }

        if (messageText == null)
        {
            Debug.LogError("UI_Message: messageText no asignado");
            return;
        }

        messagePanel.gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        Show(message, false, defaultDisplayDuration);
    }

    public void Show(string message, bool persistent)
    {
        Show(message, persistent, defaultDisplayDuration);
    }

    public void Show(string message, bool persistent, float duration)
    {
        if (messagePanel == null || messageText == null) return;

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        messageText.text = message;
        messagePanel.gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(messagePanel);

        Vector2 textSize = messageText.GetPreferredValues(message);
        messagePanel.sizeDelta = textSize + padding;

        if (!persistent)
            hideCoroutine = StartCoroutine(HideAfterDelay(duration));
    }

    public void Hide()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        if (messagePanel != null)
            messagePanel.gameObject.SetActive(false);
    }

    private IEnumerator HideAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (messagePanel != null)
            messagePanel.gameObject.SetActive(false);

        hideCoroutine = null;
    }
}