using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManagerOso : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Image symbolImage;

    public void ShowPhase(TeddyDialogueData.DialoguePhase phase)
    {
        // Activar panel
        dialoguePanel.SetActive(true);

        // Mostrar texto
        dialogueText.text = phase.text;

        // Mostrar símbolo si existe
        if (phase.symbolSprite != null)
        {
            symbolImage.sprite = phase.symbolSprite;
            symbolImage.gameObject.SetActive(true);
        }
        else
        {
            symbolImage.gameObject.SetActive(false);
        }
    }
}