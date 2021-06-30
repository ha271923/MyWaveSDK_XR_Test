using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollbarControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool ShowScrollbarWhenPoint = true;

    private ScrollRect scrollRect;
    private Image barImage;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        barImage = scrollRect.verticalScrollbar.GetComponentInChildren<Image>();
        barImage.enabled = false;

        if (scrollRect.content.GetComponent<RectTransform>().rect.height <= scrollRect.GetComponent<RectTransform>().rect.height)
        {
            enabled = false;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ShowScrollbarWhenPoint)
        {
            barImage.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        barImage.enabled = false;
    }
}
