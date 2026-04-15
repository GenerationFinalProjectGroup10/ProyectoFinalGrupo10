using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{

    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Sprite keySprite;
    public Sprite coinSprite;
    public Sprite clueSprite;
    public Sprite timeSprite;

    public Transform pfItemWorld;

}