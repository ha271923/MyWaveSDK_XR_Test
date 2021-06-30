using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIntDropdownHandler : UIIntHandler
{
    [SerializeField] protected LensUITMPDropdown dropdown;

    protected override void OnEnable()
    {
        base.OnEnable();

        dropdown.value = currentVariableHandler.Handler.CurrentValue;
        dropdown.onValueChanged.Invoke(currentVariableHandler.Handler.CurrentValue);
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    protected virtual void OnDropdownValueChanged(int value)
    {
        if(value != currentVariableHandler.Handler.CurrentValue)
        {
            currentVariableHandler.Handler.SetValue(value);
            eventHandler.Trigger1(currentVariableHandler.Handler.CurrentValue);
        }
    }

    protected override void OnReceiverValueChanged()
    {
        if (variableReceiver.Handler.CurrentValue != currentVariableHandler.Handler.CurrentValue)
        {
            currentVariableHandler.SetValue(variableReceiver.Handler.CurrentValue);
            dropdown.value = variableReceiver.Handler.CurrentValue;
            dropdown.onValueChanged.Invoke(currentVariableHandler.Handler.CurrentValue);
        }
        //Debug.Log(gameObject.name + "Receive value: " + variableReceiver.Handler.CurrentValue);
    }
}
