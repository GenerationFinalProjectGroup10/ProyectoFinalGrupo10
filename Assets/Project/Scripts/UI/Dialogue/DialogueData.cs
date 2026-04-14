using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    [Header("Texto del botón")]
    public string optionText;

    [Header("Respuesta")]
    [TextArea(2,4)]
    public string responseText;

    [Header("Quién responde")]
    public string responseCharacterName;
    public Sprite responseCharacterIcon;

    [Header("Estilo")]
    public Color responseNameColor = Color.white;

    [Header("Audio")]
    public AudioClip responseVoiceClip;
}

[System.Serializable]
public class DialogueNode
{
    [Header("Personaje")]
    public string characterName;
    public Sprite characterIcon;

    [Header("Estilo")]
    public Color nameColor = Color.white;

    [Header("Diálogo")]
    [TextArea(3, 5)]
    public string dialogueText;

    [Header("Opciones")]
    public DialogueOption[] options;

    [Header("Audio")]
    public AudioClip voiceClip;
}