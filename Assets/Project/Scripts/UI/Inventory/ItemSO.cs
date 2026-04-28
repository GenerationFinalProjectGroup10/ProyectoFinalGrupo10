using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type;
    [TextArea] public string pickupMessage;
}

public enum ItemType
{
    Key,
    FramePart,
    ClockPart,
    Frame,
    Bear
}