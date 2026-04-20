// PuzzleReward.cs
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PuzzleReward : MonoBehaviour
{
    [Header("UI")]
    public GameObject rewardPanel;
    public TextMeshProUGUI narrativeText;
    public GameObject guideItem;     // el ítem "guía" que se desbloquea

    [Header("Events — conecta aquí tu sistema de narrativa")]
    public UnityEvent OnPuzzleSolved;
    public UnityEvent OnNarrativeUnlocked;

    [Header("Audio")]
    public AudioClip solvedSound;

    public void ActivateReward(string narrative)
    {
        StartCoroutine(ShowReward(narrative));
    }

    private IEnumerator ShowReward(string narrative)
    {
        // Sonido de resolución
        if (solvedSound != null)
            AudioSource.PlayClipAtPoint(solvedSound, Camera.main.transform.position);

        // Activar panel de recompensa
        rewardPanel.SetActive(true);
        narrativeText.text = "";

        // Escribir narrativa poco a poco
        foreach (char c in narrative)
        {
            narrativeText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(1f);

        // Desbloquear ítem guía
        if (guideItem != null)
        {
            guideItem.SetActive(true);
            // Aquí llamas a tu inventario: Inventory.Instance.AddItem(guideItemData);
        }

        // Disparar eventos de narrativa (conecta en el Inspector)
        OnPuzzleSolved?.Invoke();
        yield return new WaitForSeconds(0.5f);
        OnNarrativeUnlocked?.Invoke();
    }
}