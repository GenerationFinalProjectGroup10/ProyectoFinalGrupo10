using UnityEngine;

public class TeddyBearPuzzle : MonoBehaviour
{
    public TeddyDialogueData dialogueData;

    public DialogueManagerOso dialogueManager;
    public PuzzleReward puzzleReward;
    public GameObject codeInputPanel;

    private int currentPhase = 0;

    public void OnPlayerInteract()
    {
        if (dialogueData == null) return;

        if (currentPhase < dialogueData.phases.Length)
        {
            dialogueManager.ShowPhase(dialogueData.phases[currentPhase]);

            if (dialogueData.phases[currentPhase].revealCode)
            {
                codeInputPanel.SetActive(true);
            }

            currentPhase++;
        }
    }

    // 🔥 CORREGIDO (antes era CheckCode)
    public void OnCodeSubmitted(string input)
    {
        if (input == dialogueData.secretCode)
        {
            Debug.Log("Codigo correcto");

            puzzleReward.ActivateReward(dialogueData.rewardNarrativeText);
        }
        else
        {
            Debug.Log("Codigo incorrecto");
        }
    }
}