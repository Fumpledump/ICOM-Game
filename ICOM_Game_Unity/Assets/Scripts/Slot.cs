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

            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
        UpdateSlotInfo(currentSlot, entries[currentSlot]);
    }

    private void UpdateSlotInfo(int slotIndex, InputEntry entry)
    {
        // Add Button
        Button collectionBtn = slots[slotIndex].GetComponent<Button>();
        collectionBtn.onClick.RemoveAllListeners();
        collectionBtn.onClick.AddListener(() =>
        {
            inputHandler.OpenCollectionPage(entries[slotIndex]);
            inputHandler.curCollectionIndex = entries[slotIndex].Index;
        });

        /*
        // grid image showing
        Image collectionImage = slots[slotIndex].GetComponent<Image>();
        inputHandler.LoadRawImage(entry, collectionImage, slotIndex);
        inputHandler.LoadSprite(collectionImage, slotIndex);
        */
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
