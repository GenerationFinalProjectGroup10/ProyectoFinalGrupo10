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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine =
            StartCoroutine(TypeText(node.dialogueText, node.voiceClip));
    }

    IEnumerator TypeText(string text, AudioClip clip)
    {
        dialogueText.text = "";

        StartVoice(clip);

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        StopVoice();

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
        // NIÑA / PLAYER
        nameText.text = currentOption.responseCharacterName;
        nameText.color = currentOption.responseNameColor;

        speakerIcon.sprite = currentOption.responseCharacterIcon;
        speakerIcon.enabled = currentOption.responseCharacterIcon != null;

        dialogueText.text = "";

        StartVoice(nodes[currentNode].voiceClip);

        foreach (char letter in currentOption.optionText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        StopVoice();

        yield return new WaitForSeconds(1f);

        // RESPUESTA PERSONAJE PRINCIPAL
        DialogueNode node = nodes[currentNode];

        nameText.text = node.characterName;
        nameText.color = node.nameColor;

        speakerIcon.sprite = node.characterIcon;
        speakerIcon.enabled = node.characterIcon != null;

        dialogueText.text = "";

        StartVoice(node.voiceClip);

        foreach (char letter in currentOption.responseText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        StopVoice();

        yield return new WaitForSeconds(1.5f);

        NextNode();
    }

    void StartVoice(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    void StopVoice()
    {
        if (audioSource != null)
            audioSource.Stop();
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

    public void HideDialogueInstant()
    {
        StopVoice();

        if (dialogueCanvasGroup != null)
            dialogueCanvasGroup.alpha = 0f;
    }

    void EndDialogue()
    {
        StopVoice();

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