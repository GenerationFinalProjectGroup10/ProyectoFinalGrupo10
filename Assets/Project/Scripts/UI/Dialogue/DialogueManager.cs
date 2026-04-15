using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image speakerIcon;

    public GameObject choicesPanel;
    public Button[] choiceButtons;

    [Header("Canvas Fade UI")]
    public CanvasGroup dialogueCanvasGroup;
    public float fadeDuration = 2f;
    public float delayBeforeStart = 2f;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Typing")]
    public float typingSpeed = 0.03f;

    [Header("Data")]
    public DialogueNode[] nodes;

    [Header("Scene Control")]
    public SceneFader sceneFader;
    public string nextSceneName = "Mundo1";

    private int currentNode = 0;
    private Coroutine typingCoroutine;
    private bool waitingForChoice = false;

    private string currentFullText;
    private DialogueOption currentOption;

    private bool isInResponse = false;

    void Start()
    {
        choicesPanel.SetActive(false);

        nameText.text = "";
        dialogueText.text = "";

        speakerIcon.sprite = null;
        speakerIcon.enabled = false;

        if (dialogueCanvasGroup != null)
            dialogueCanvasGroup.alpha = 0;

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            if (dialogueCanvasGroup != null)
                dialogueCanvasGroup.alpha = Mathf.Lerp(0, 1, time / fadeDuration);

            yield return null;
        }

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

        isInResponse = false;

        dialogueText.text = "";

        nameText.text = node.characterName;
        nameText.color = node.nameColor;

        speakerIcon.sprite = node.characterIcon;
        speakerIcon.enabled = true;

        choicesPanel.SetActive(false);
        waitingForChoice = false;

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
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

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

                    TMP_Text buttonText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
                    buttonText.text = node.options[i].optionText;

                    choiceButtons[i].onClick.RemoveAllListeners();

                    int capturedIndex = i;
                    choiceButtons[i].onClick.AddListener(() => SelectOption(capturedIndex));
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

        isInResponse = true;

        // 👶 niña habla
        nameText.text = currentOption.responseCharacterName;
        nameText.color = currentOption.responseNameColor;

        speakerIcon.sprite = currentOption.responseCharacterIcon;
        speakerIcon.enabled = true;

        currentFullText = currentOption.optionText;

        typingCoroutine = StartCoroutine(TypePlayerLine(currentFullText));
    }

    IEnumerator TypePlayerLine(string text)
    {
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

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
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

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
        if (sceneFader != null)
            sceneFader.FadeToScene(nextSceneName);
    }
}