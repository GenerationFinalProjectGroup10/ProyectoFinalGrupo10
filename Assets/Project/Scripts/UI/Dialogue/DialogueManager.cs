using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image speakerIcon;

    public GameObject choicesPanel;
    public Button[] choiceButtons;

    [Header("Canvas Fade")]
    public CanvasGroup dialogueCanvasGroup;
    public float fadeDuration = 2f;
    public float delayBeforeStart = 2f;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Typing")]
    public float typingSpeed = 0.03f;

    [Header("Data")]
    public DialogueNode[] nodes;

    private int currentNode = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool waitingForChoice = false;

    private string currentFullText;
    private DialogueOption currentOption;

    void Start()
    {
        choicesPanel.SetActive(false);

        // 🔥 limpiar UI completamente
        nameText.text = "";
        dialogueText.text = "";

        // 🔥 evitar icono blanco
        speakerIcon.sprite = null;
        speakerIcon.enabled = false;

        if (dialogueCanvasGroup != null)
            dialogueCanvasGroup.alpha = 0;

        StartCoroutine(IntroSequence());
    }

    void Update()
    {
        if (waitingForChoice) return;

        if (Input.GetKeyDown(KeyCode.Space) && isTyping)
        {
            CompleteText();
        }

        if (Input.GetMouseButtonDown(0) && isTyping)
        {
            CompleteText();
        }
    }

    void CompleteText()
    {
        StopCoroutine(typingCoroutine);
        dialogueText.text = currentFullText;
        isTyping = false;
    }

    IEnumerator IntroSequence()
    {
        // 🔥 asegurar todo limpio antes del fade
        nameText.text = "";
        dialogueText.text = "";
        speakerIcon.enabled = false;

        yield return new WaitForSeconds(delayBeforeStart);

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            if (dialogueCanvasGroup != null)
                dialogueCanvasGroup.alpha = Mathf.Lerp(0, 1, time / fadeDuration);

            yield return null;
        }

        if (dialogueCanvasGroup != null)
            dialogueCanvasGroup.alpha = 1;

        yield return new WaitForSeconds(0.3f);

        StartDialogue();
    }

    void StartDialogue()
    {
        currentNode = 0;
        ShowNode();
    }

    void ShowNode()
    {
        DialogueNode node = nodes[currentNode];

        dialogueText.text = "";

        nameText.text = node.characterName;
        nameText.color = node.nameColor;

        // 🔥 activar icono SOLO cuando ya hay datos
        speakerIcon.sprite = node.characterIcon;
        speakerIcon.enabled = true;

        choicesPanel.SetActive(false);
        waitingForChoice = false;

        // 🔊 audio madre
        if (node.voiceClip != null && audioSource != null)
        {
            audioSource.clip = node.voiceClip;
            audioSource.Play();
        }

        currentFullText = node.dialogueText;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentFullText));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        ShowChoices();
    }

    void ShowChoices()
    {
        DialogueNode node = nodes[currentNode];

        if (node.options != null && node.options.Length > 0)
        {
            waitingForChoice = true;
            choicesPanel.SetActive(true);

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < node.options.Length)
                {
                    choiceButtons[i].gameObject.SetActive(true);

                    int index = i;

                    TMP_Text buttonText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
                    buttonText.text = node.options[i].optionText;

                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => SelectOption(index));
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void SelectOption(int optionIndex)
    {
        DialogueNode node = nodes[currentNode];
        currentOption = node.options[optionIndex];

        waitingForChoice = false;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        choicesPanel.SetActive(false);

        // 👶 NIÑA HABLA (lo que elegiste)
        nameText.text = currentOption.responseCharacterName;
        nameText.color = currentOption.responseNameColor;

        speakerIcon.sprite = currentOption.responseCharacterIcon;
        speakerIcon.enabled = true;

        currentFullText = currentOption.optionText;

        typingCoroutine = StartCoroutine(TypePlayerLine(currentFullText));
    }

    IEnumerator TypePlayerLine(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        yield return new WaitForSeconds(1f);

        ShowMotherResponse();
    }

    void ShowMotherResponse()
    {
        DialogueNode node = nodes[currentNode];

        nameText.text = node.characterName;
        nameText.color = node.nameColor;

        speakerIcon.sprite = node.characterIcon;
        speakerIcon.enabled = true;

        // 🔊 audio madre
        if (node.voiceClip != null && audioSource != null)
        {
            audioSource.clip = node.voiceClip;
            audioSource.Play();
        }

        currentFullText = currentOption.responseText;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeResponse(currentFullText));
    }

    IEnumerator TypeResponse(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        yield return new WaitForSeconds(1.5f);

        NextNode();
    }

    void NextNode()
    {
        currentNode++;

        if (currentNode < nodes.Length)
            ShowNode();
        else
            EndDialogue();
    }

    void EndDialogue()
    {
        Debug.Log("Fin del diálogo - Cambiando a escena 2...");
        // Opcional: esperar un poco antes de cambiar (para que se vea el último texto)
        StartCoroutine(CambioConDelay());
    }

    IEnumerator CambioConDelay()
    {
        yield return new WaitForSeconds(1f); // espera 1 segundo (ajústalo)
        SceneManager.LoadScene("Mundo1");
    }
}