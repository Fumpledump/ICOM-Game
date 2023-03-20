using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    List<InputEntry> entries;
    List<GameObject> collectionSlots;
    List<Sprite> collectionSprite;
    int curIndex;

    void Awake()
    {
        List<InputEntry> entries = new List<InputEntry>();
        List<GameObject> collectionSlots = new List<GameObject>();
        List<Sprite> collectionSprite = new List<Sprite>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
