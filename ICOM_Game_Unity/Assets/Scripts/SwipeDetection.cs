using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeDetection : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 intialPosition;
    private Slot slotHandler;

    private void Start()
    {
        slotHandler = this.gameObject.GetComponent<Slot>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = new Vector2(transform.localPosition.x+eventData.delta.x, transform.localPosition.y);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        intialPosition = transform.localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (intialPosition.x > transform.localPosition.x)
        {
            slotHandler.ShiftSlotsRight();
        }
        else
        {
            slotHandler.ShiftSlotsLeft();
        }
        transform.localPosition = intialPosition;
    }
}
