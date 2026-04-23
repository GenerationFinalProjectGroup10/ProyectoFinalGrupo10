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

    [Header("Sequence Control")]
    public bool usePauseSystem = false;

    [Header("Fade Control")]
    public bool pauseEndFade = false;

    [HideInInspector] public bool waitingExternalResume = false;

    private int currentNode = 0;
    private Coroutine typingCoroutine;

    private DialogueOption currentOption;

    void Start()
    {
        choicesPanel.SetActive(false);

        nameText.text = "";
        dialogueText.text = "";

        speakerIcon.sprite = null;
        speakerIcon.enabled = false;

        if (dialogueCanvasGroup != null)
            dialogueCanvasGroup.alpha = 0f;

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            if (dialogueCanvasGroup != null)
                dialogueCanvasGroup.alpha =
                    Mathf.Lerp(0f, 1f, time / fadeDuration);

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
        if (currentNode < 0 || currentNode >= nodes.Length)
        {
            EndDialogue();
            return;
        }

        DialogueNode node = nodes[currentNode];

        choicesPanel.SetActive(false);

        if (dialogueCanvasGroup != null)
            dialogueCanvasGroup.alpha = 1f;

        nameText.text = node.characterName;
        nameText.color = node.nameColor;

        speakerIcon.sprite = node.characterIcon;
        speakerIcon.enabled = node.characterIcon != null;

        if (node.voiceClip != null && audioSource != null)
        {
            audioSource.clip = node.voiceClip;
            audioSource.Play();
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine =
            StartCoroutine(TypeText(node.dialogueText));
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

        if (node.options == null || node.options.Length == 0)
        {
            StartCoroutine(AutoNextNode());
            return;
        }

        choicesPanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < node.options.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);

                TMP_Text txt =
                    choiceButtons[i].GetComponentInChildren<TMP_Text>();

                txt.text = node.options[i].optionText;

                choiceButtons[i].onClick.RemoveAllListeners();

                int id = i;
                choiceButtons[i].onClick.AddListener(() =>
                    SelectOption(id));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    IEnumerator AutoNextNode()
    {
        yield return new WaitForSeconds(1f);
        NextNode();
    }

    void SelectOption(int optionIndex)
    {
        DialogueNode node = nodes[currentNode];
        currentOption = node.options[optionIndex];

        choicesPanel.SetActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine =
            StartCoroutine(TypeOptionFlow());
    }

    IEnumerator TypeOptionFlow()
    {
        dialogueText.text = currentOption.optionText;
        yield return new WaitForSeconds(1f);

        dialogueText.text = currentOption.responseText;
        yield return new WaitForSeconds(1.5f);

        NextNode();
    }

    void NextNode()
    {
        if (usePauseSystem &&
            (currentNode == 0 || currentNode == 1))
        {
            waitingExternalResume = true;

            if (dialogueCanvasGroup != null)
                dialogueCanvasGroup.alpha = 0f;

            return;
        }

        currentNode++;

        if (currentNode < nodes.Length)
            ShowNode();
        else
            EndDialogue();
    }

    public void ContinueDialogue()
    {
        waitingExternalResume = false;

        currentNode++;

        if (currentNode < nodes.Length)
            ShowNode();
        else
            EndDialogue();
    }

    public void ForceNextNode()
    {
        waitingExternalResume = false;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentNode++;

        if (currentNode < nodes.Length)
            ShowNode();
        else
            EndDialogue();
    }

    void EndDialogue()
    {
        if (pauseEndFade)
        {
            waitingExternalResume = true;

            if (dialogueCanvasGroup != null)
                dialogueCanvasGroup.alpha = 0f;

            return;
        }

        DoFadeNow();
    }

    public void DoFadeNow()
    {
        if (sceneFader != null)
            sceneFader.FadeToScene(nextSceneName);
    }
}