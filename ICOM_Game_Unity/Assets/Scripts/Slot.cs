using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public InputHandler inputHandler;
    public List<GameObject> slots = new List<GameObject>();
    public List<GameObject> positions = new List<GameObject>();
    public List<InputEntry> entries = new List<InputEntry>();
    [SerializeField] int currentSlot = 0;

    private void Start()
    {
        inputHandler = InputHandler.instance;
        UpdateSlots();
    }
    public void UpdateSlots()
    {
        // Set Slot Positions
        switch (slots.Count)
        {
            case 1: // In the Middle
                slots[0].transform.position = positions[1].transform.position; // Set to 2nd Position

                DeactivatePositions();
                positions[1].SetActive(true);
                break;
            case 2: // Close Togeter in Middle
                slots[0].transform.position = positions[3].transform.position; // Set to 4th Position
                slots[1].transform.position = positions[4].transform.position; // Set to 5th Position

                DeactivatePositions();
                positions[3].SetActive(true);
                positions[4].SetActive(true);
                break;
            case 3: // Set Of 3
                slots[0].transform.position = positions[0].transform.position;
                slots[1].transform.position = positions[1].transform.position;
                slots[2].transform.position = positions[2].transform.position;

                DeactivatePositions();
                positions[0].SetActive(true);
                positions[1].SetActive(true);
                positions[2].SetActive(true);
                break;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            UpdateSlotInfo(i, entries[i]);
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

        if (entry.Favorited)
        {
            slots[slotIndex].transform.Find("Favorited").gameObject.SetActive(true);
        }
        else
        {
            slots[slotIndex].transform.Find("Favorited").gameObject.SetActive(false);
        }
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

    public void DeactivatePositions()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            positions[i].SetActive(false);
        }
    }
}
