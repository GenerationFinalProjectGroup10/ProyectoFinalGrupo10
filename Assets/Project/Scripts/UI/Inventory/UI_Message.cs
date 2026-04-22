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

    [Header("Durations")]
    [SerializeField] private float defaultTemporaryDuration = 2f;

    [Header("Layout")]
    [SerializeField] private Vector2 padding = new Vector2(30f, 18f);

    private Coroutine temporaryCoroutine;
    private bool interactionMessageActive;
    private string interactionMessage;

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

    public void ShowInteraction(string message)
    {
        if (messagePanel == null || messageText == null) return;

        interactionMessageActive = true;
        interactionMessage = message;

        if (temporaryCoroutine != null)
        {
            StopCoroutine(temporaryCoroutine);
            temporaryCoroutine = null;
        }

        messageText.text = message;
        messagePanel.gameObject.SetActive(true);
        ResizePanel(message);
    }

    public void HideInteraction()
    {
        interactionMessageActive = false;
        interactionMessage = "";

        if (temporaryCoroutine == null)
        {
            if (messagePanel != null)
                messagePanel.gameObject.SetActive(false);
        }
    }

    public void ShowTemporary(string message)
    {
        ShowTemporary(message, defaultTemporaryDuration);
    }

    public void ShowTemporary(string message, float duration)
    {
        if (messagePanel == null || messageText == null) return;

        if (temporaryCoroutine != null)
        {
            StopCoroutine(temporaryCoroutine);
            temporaryCoroutine = null;
        }

        messageText.text = message;
        messagePanel.gameObject.SetActive(true);
        ResizePanel(message);

        temporaryCoroutine = StartCoroutine(HideTemporaryAfterDelay(duration));
    }

    private void ResizePanel(string message)
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(messagePanel);

        Vector2 textSize = messageText.GetPreferredValues(message);
        messagePanel.sizeDelta = textSize + padding;
    }

    private IEnumerator HideTemporaryAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        temporaryCoroutine = null;

        if (interactionMessageActive)
        {
            messageText.text = interactionMessage;
            messagePanel.gameObject.SetActive(true);
            ResizePanel(interactionMessage);
        }
        else if (messagePanel != null)
        {
            messagePanel.gameObject.SetActive(false);
        }
    }
}