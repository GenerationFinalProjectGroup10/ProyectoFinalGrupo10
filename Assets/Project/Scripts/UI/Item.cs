using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{

    public enum ItemType
    {
        Key,
        Coin,
        Time,
        Clue,
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.Key: return ItemAssets.Instance.keySprite;
            case ItemType.Clue: return ItemAssets.Instance.coinSprite;
            case ItemType.Time: return ItemAssets.Instance.clueSprite;
            case ItemType.Coin: return ItemAssets.Instance.timeSprite;
        }
    }
}