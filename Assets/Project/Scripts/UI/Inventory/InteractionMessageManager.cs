using UnityEngine;

public class InteractionMessageManager : MonoBehaviour
{
    public static InteractionMessageManager Instance { get; private set; }

    private string activeMessage = "";
    private int activePriority = int.MinValue;
    private bool activeTemporary;
    private float temporaryEndTime = -1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (activeTemporary && Time.time >= temporaryEndTime)
        {
            activeTemporary = false;
            activeMessage = "";
            activePriority = int.MinValue;
            UI_Message.Instance?.HideInteraction();
        }
    }

    public void ShowInteraction(string message, int priority)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        if (priority < activePriority) return;

        activeMessage = message;
        activePriority = priority;
        activeTemporary = false;

        UI_Message.Instance?.ShowInteraction(message);
    }

    public void ShowTemporary(string message, float duration, int priority)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        if (priority < activePriority) return;

        activeMessage = "";
        activePriority = priority;
        activeTemporary = true;
        temporaryEndTime = Time.time + duration;

        UI_Message.Instance?.ShowTemporary(message, duration);
    }

    public void ClearInteraction(int priority)
    {
        if (priority < activePriority) return;

        if (!activeTemporary)
        {
            activeMessage = "";
            activePriority = int.MinValue;
            UI_Message.Instance?.HideInteraction();
        }
    }

    public void ForceClear()
    {
        activeMessage = "";
        activePriority = int.MinValue;
        activeTemporary = false;
        UI_Message.Instance?.HideInteraction();
    }
}