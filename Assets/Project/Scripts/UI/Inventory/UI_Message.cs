using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class UI_Message : MonoBehaviour
{
    public static UI_Message Instance;

    [Header("Prompt UI")]
    [SerializeField] private RectTransform promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Durations")]
    [SerializeField] private float defaultTemporaryDuration = 5f;

    [Header("Layout")]
    [SerializeField] private Vector2 padding = new Vector2(30f, 18f);

    private Coroutine hideCoroutine;
    private Coroutine queuedCoroutine;
    private string currentMessage;
    private bool locked;
    private bool isTemporary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (promptPanel != null) promptPanel.gameObject.SetActive(false);
    }

    public void ShowInteraction(string message)
    {
        if (promptPanel == null || promptText == null) return;
        if (locked || isTemporary) return;
        if (promptPanel.gameObject.activeSelf && currentMessage == message) return;

        StopHide();
        currentMessage = message;
        promptText.text = message;
        promptPanel.gameObject.SetActive(true);
        ResizePanel(message);
    }

    public void ShowInteractionQueued(string message, float delay)
    {
        if (promptPanel == null || promptText == null) return;

        if (queuedCoroutine != null)
            StopCoroutine(queuedCoroutine);

        queuedCoroutine = StartCoroutine(ShowAfterDelay(message, delay, 0f));
    }

    public void ShowTemporary(string message)
    {
        ShowTemporary(message, defaultTemporaryDuration);
    }

    public void ShowTemporary(string message, float duration)
    {
        if (promptPanel == null || promptText == null) return;

        if (queuedCoroutine != null)
        {
            StopCoroutine(queuedCoroutine);
            queuedCoroutine = null;
        }

        StopHide();
        isTemporary = true;
        currentMessage = message;
        promptText.text = message;
        promptPanel.gameObject.SetActive(true);
        ResizePanel(message);
        hideCoroutine = StartCoroutine(HideAfter(duration));
    }

    public void HideInteraction()
    {
        if (locked || isTemporary) return;
        StopHide();
        currentMessage = null;
        if (promptPanel != null) promptPanel.gameObject.SetActive(false);
    }

    public void LockPrompt()
    {
        locked = true;
    }

    public void UnlockPrompt()
    {
        locked = false;
    }

    public void ClearPrompt()
    {
        currentMessage = null;
        isTemporary = false;
        if (promptPanel != null) promptPanel.gameObject.SetActive(false);
    }

    private IEnumerator ShowAfterDelay(string message, float delay, float hideAfter)
    {
        yield return new WaitForSeconds(delay);
        isTemporary = false;
        currentMessage = message;
        promptText.text = message;
        promptPanel.gameObject.SetActive(true);
        ResizePanel(message);
        queuedCoroutine = null;

        if (hideAfter > 0f)
        {
            StopHide();
            hideCoroutine = StartCoroutine(HideAfter(hideAfter));
        }
    }

    private IEnumerator HideAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        hideCoroutine = null;
        isTemporary = false;
        if (promptPanel != null)
            promptPanel.gameObject.SetActive(false);
    }

    private void StopHide()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    private void ResizePanel(string message)
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(promptPanel);
        promptPanel.sizeDelta = promptText.GetPreferredValues(message) + padding;
    }
}