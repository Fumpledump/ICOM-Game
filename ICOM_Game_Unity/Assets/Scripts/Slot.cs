using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public InputHandler inputHandler;
    public List<GameObject> slots = new List<GameObject>();
    public List<InputEntry> entries = new List<InputEntry>();
    [SerializeField] int currentSlot = 0;

    private void Start()
    {
        inputHandler = InputHandler.instance;
        UpdateSlots();
    }
    public void UpdateSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (currentSlot == i)
            {
                slots[i].gameObject.SetActive(true);
                UpdateSlotInfo(currentSlot, entries[currentSlot]);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateSlotInfo(int slotIndex, InputEntry entry)
    {
        // Add Button
        Button collectionBtn = slots[slotIndex].GetComponent<Button>();
        collectionBtn.onClick.RemoveAllListeners();
        collectionBtn.onClick.AddListener(() =>
        {
            inputHandler.OpenCollectionPage(entry);
            inputHandler.curCollectionIndex = entry.Index;
        });

        /*
        // grid image showing
        Image collectionImage = slots[slotIndex].GetComponent<Image>();
        inputHandler.LoadRawImage(entry, collectionImage, slotIndex);
        inputHandler.LoadSprite(collectionImage, slotIndex);
        */

        Image collectionImage = slots[slotIndex].GetComponent<Image>();
        if (inputHandler != null)
        {
            inputHandler.LoadSprite(collectionImage, entry.Index);
        }
        //collectionImage.sprite = inputHandler.collectionSprite[entry.Index];
        //Debug.Log(entry.Index);
    }

    // Change Main Slot to the next slot
    public void ShiftSlotsRight()
    {
        currentSlot++;
        if (currentSlot > slots.Count - 1)
        {
            currentSlot = 0;
        }
        UpdateSlots();
    }

    // Change Main Slot to the previous slot
    public void ShiftSlotsLeft()
    {
        currentSlot--;
        if (currentSlot < 0)
        {
            currentSlot = slots.Count - 1;
        }
        UpdateSlots();
    }
}
