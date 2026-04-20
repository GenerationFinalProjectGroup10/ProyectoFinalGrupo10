using UnityEngine;
using TMPro;
using System.Collections;

public class UI_Message : MonoBehaviour
{
    public static UI_Message Instance;

    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float displayDuration = 2f;

    private Coroutine hideCoroutine;
    private bool isShowingTempMessage = false;

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

        messagePanel.SetActive(false);
    }

    public void Show(string message, bool persistent = false)
    {
        if (isShowingTempMessage && persistent) return;

        messageText.text = message;
        messagePanel.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        if (!persistent)
        {
            isShowingTempMessage = true;
            hideCoroutine = StartCoroutine(HideAfterDelay());
        }
    }

    public void Hide()
    {
        if (isShowingTempMessage) return;

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        messagePanel.SetActive(false);
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        messagePanel.SetActive(false);
        isShowingTempMessage = false;
    }
}