using UnityEngine;

[CreateAssetMenu(fileName = "TeddyDialogueData", menuName = "Puzzle/Teddy Dialogue Data")]
public class TeddyDialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialoguePhase
    {
        [TextArea(2, 5)]
        public string text;
        public AudioClip voiceClip;
        public Sprite symbolSprite;
        public float displayDuration = 4f;
        public bool revealCode;
    }

    public DialoguePhase[] phases;
    public string secretCode = "IXATO";
    public string rewardNarrativeText;
}