using UnityEngine;

public class SceneWelcomeMessage : MonoBehaviour
{
    [SerializeField] private string message = "¿Estás desorientada?. ¡Encontrar el ojo perdido del oso te podría ser útil!";
    [SerializeField] private float duration = 10f;

    private void Start()
    {
        UI_Message.Instance?.ShowTemporary(message, duration);
    }
}