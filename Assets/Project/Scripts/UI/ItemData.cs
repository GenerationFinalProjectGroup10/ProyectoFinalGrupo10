using UnityEngine;

public enum ItemType { Key, Consumable, Weapon, Quest }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identificación")]
    public string itemID;           // ID único, ej: "key_door_01"
    public string itemName;
    [TextArea] public string description;

    [Header("Visual")]
    public Sprite icon;
    public GameObject worldPrefab;  // El objeto 3D en la escena

    [Header("Configuración")]
    public ItemType type;
    public bool isStackable = false;
    public int maxStack = 1;
}