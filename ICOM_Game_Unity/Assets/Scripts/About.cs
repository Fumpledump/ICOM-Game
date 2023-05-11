using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class About : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public List<GameObject> pages;
    public int currentPage;

    public Vector3 intialPosition;

    private void Start()
    {
        currentPage = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + eventData.delta.y);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        intialPosition = transform.localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (intialPosition.y < transform.localPosition.y && currentPage != pages.Count - 1)
        {
            int page = currentPage += 1;
            SetPage(page);
            Debug.Log("Next Page");
        }
        else if(intialPosition.y > transform.localPosition.y && currentPage != 0)
        {
            int page = currentPage -= 1;
            SetPage(page);
            Debug.Log("Previous Page");
        }
        transform.localPosition = intialPosition;
    }

    public void SetPage(int pageIndex)
    {
        currentPage = pageIndex;
        for (int i = 0; i < pages.Count; i++)
        {
            if (i == pageIndex)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }

    }
}
