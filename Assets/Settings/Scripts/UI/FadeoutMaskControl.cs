using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FadeoutMaskControl : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public bool ControlTopMask = true;
    public GameObject TopFadeoutMask;
    public bool ControlBottomMask = false;
    public GameObject BottomFadeoutMask;

    private ScrollRect scrollRect;
    private float scrollValue;
    private bool isDragging = false;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.verticalScrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);

        if(!ControlTopMask && !ControlBottomMask)
        {
            enabled = false;
        }
    }

    public void OnScrollbarValueChanged(float value)
    {
        //Debug.Log("OnScrollbarValueChanged: " + value);
        scrollValue = value;
        if (ControlTopMask && !isDragging && value >= 1.0f && TopFadeoutMask.activeInHierarchy)
        {
            TopFadeoutMask.SetActive(false);
        }

        if (ControlBottomMask && !isDragging && value <= 0.0f && BottomFadeoutMask.activeInHierarchy)
        {
            BottomFadeoutMask.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        if (ControlTopMask && !TopFadeoutMask.activeInHierarchy)
        {
            TopFadeoutMask.SetActive(true);
        }

        if (ControlBottomMask && !BottomFadeoutMask.activeInHierarchy)
        {
            BottomFadeoutMask.SetActive(true);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        if (ControlTopMask && scrollValue >= 1.0f && TopFadeoutMask.activeInHierarchy)
        {
            TopFadeoutMask.SetActive(false);
        }

        if (ControlBottomMask && scrollValue <= 0.0f && BottomFadeoutMask.activeInHierarchy)
        {
            BottomFadeoutMask.SetActive(false);
        }
    }
}
