using UnityEngine;

public class BasementWelcomeMessage : MonoBehaviour
{
    [SerializeField] private string message = "¿Te has perdido?. ¡Encontrar el portaretrato sería útil!";
    [SerializeField] private float duration = 10f;

    private void Start()
    {
        UI_Message.Instance?.ShowTemporary(message, duration);
    }
}