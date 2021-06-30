using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIIntSliderHandler : UIIntHandler, IPointerUpHandler//, IEndDragHandler
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI SliderText;

    public bool updateValueOnEndDrag = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        //Debug.Log(gameObject.name + " Current UI value: " + currentVariableHandler.Handler.CurrentValue);
        //Debug.Log(gameObject.name + " Receive value: " + variableReceiver.Handler.CurrentValue);
        //int clamp_value = Mathf.RoundToInt(Mathf.Clamp(currentVariableHandler.Handler.CurrentValue, slider.minValue, slider.maxValue));
        //slider.value = clamp_value;
        //slider.onValueChanged.Invoke(clamp_value);
        slider.onValueChanged.AddListener(OnSliderbarValueChanged);
        //SliderText.text = clamp_value.ToString() + "%";
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        slider.onValueChanged.RemoveListener(OnSliderbarValueChanged);
    }

    private void OnSliderbarValueChanged(float value)
    {
        int ivalue = Mathf.RoundToInt(value);
        if (ivalue != currentVariableHandler.Handler.CurrentValue)
        {
            currentVariableHandler.Handler.SetValue(ivalue);
            eventHandler.Trigger1(currentVariableHandler.Handler.CurrentValue);
        }
    }

    protected override void OnReceiverValueChanged()
    {
        Debug.Log("[Settings][UIIntSliderHandler][" + gameObject.name + "] Receive value: " + variableReceiver.Handler.CurrentValue);

        if (variableReceiver.Handler.CurrentValue != currentVariableHandler.Handler.CurrentValue)
        {
            int clamp_value = Mathf.RoundToInt(Mathf.Clamp(variableReceiver.Handler.CurrentValue, slider.minValue, slider.maxValue));
            currentVariableHandler.SetValue(clamp_value);
            slider.value = clamp_value;
            slider.onValueChanged.Invoke(currentVariableHandler.Handler.CurrentValue);
            Debug.Log("[Settings][UIIntSliderHandler][" + gameObject.name + "] update slider value: " + clamp_value);
        }
    }

    public override void UpdateValue()
    {
        base.UpdateValue();

        int clamp_value = Mathf.RoundToInt(Mathf.Clamp(currentVariableHandler.Handler.CurrentValue, slider.minValue, slider.maxValue));
        slider.value = clamp_value;
        slider.onValueChanged.Invoke(clamp_value);
        SliderText.text = clamp_value.ToString() + "%";
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    if (updateValueOnEndDrag)
    //    {
    //        Debug.Log("[Settings][Slider][" + gameObject.name + "] update value on end drag");
    //        UpdateValue();
    //    }
    //}

    public void OnPointerUp(PointerEventData eventData)
    {
        if (updateValueOnEndDrag)
        {
            Debug.Log("[Settings][Slider][" + gameObject.name + "] update value on end drag");
            UpdateValue();
        }
    }
}