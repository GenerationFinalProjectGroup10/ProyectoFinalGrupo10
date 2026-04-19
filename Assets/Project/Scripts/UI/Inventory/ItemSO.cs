using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type;
}

public enum ItemType
{
    Key,
    CluePart,
    Tool,
    Potion
}