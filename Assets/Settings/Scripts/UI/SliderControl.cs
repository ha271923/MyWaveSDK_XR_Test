using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
    public float VerticalThreshold = 5.0f;
    public float HorizontalThreshold = 0.25f;
    [SerializeField] private Slider Sliderbar;
    [SerializeField] private TextMeshProUGUI SliderText;
    [SerializeField] private ScrollRect ScrollRect;

    private float sliderValue;

    private Vector3 pointerStartWorldCursor;
    private Vector3 barStartPos;
    private bool isDragging = false;
    private bool isDraggingScroll = false;

    private void Awake()
    {
        Sliderbar.onValueChanged.AddListener(OnSliderbarValueChanged);
    }

    private void OnSliderbarValueChanged(float value)
    {
        sliderValue = value;
        SliderText.text = sliderValue.ToString() + "%";
        //Debug.Log(gameObject.name + " slider text: " + SliderText.text);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldCursor;
        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out worldCursor))
            return;

        var worldPointerDelta = worldCursor - pointerStartWorldCursor;

        //Check if is dragging scroll
        if (!isDraggingScroll && Mathf.Abs(worldPointerDelta.y) > VerticalThreshold * 0.01f) // change unit from cm to m
        {
            isDraggingScroll = true;
        }

        if (isDraggingScroll)
        {
            if (ScrollRect != null)
            {
                ScrollRect.OnDrag(eventData);
            }
            isDragging = false;
            return;
        }


        //Check is start to drag progress bar
        if (!isDragging && Mathf.Abs(worldPointerDelta.x) > HorizontalThreshold * 0.01f)
        {
            isDragging = true;
        }

        if (!isDragging || isDraggingScroll) return;

        PointerEventData data = eventData;

        Vector3 vector = new Vector3(barStartPos.x + worldPointerDelta.x, barStartPos.y, barStartPos.z);
        data.position = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, vector);

        Sliderbar.OnDrag(data);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            ScrollRect.OnBeginDrag(eventData);
        }

        pointerStartWorldCursor = Vector3.zero;
        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out pointerStartWorldCursor))
            return;

        barStartPos = Sliderbar.handleRect.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            ScrollRect.OnEndDrag(eventData);
        }

        isDragging = false;
        isDraggingScroll = false;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (ScrollRect != null)
        {
            ScrollRect.OnInitializePotentialDrag(eventData);
        }
    }
}
