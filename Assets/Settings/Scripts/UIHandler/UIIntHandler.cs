using HTC.UnityPlugin.CommonEventVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIntHandler : MonoBehaviour
{
    [SerializeField] protected CommonEventAssetInt eventHandler;
    [SerializeField] protected CommonVariableAssetInt currentVariableHandler;
    [SerializeField] protected CommonVariableAssetInt variableReceiver;

    protected virtual void OnEnable()
    {
        UpdateValue();
        if(variableReceiver == null) return;
        if(variableReceiver.Handler == null) return;
        variableReceiver.Handler.OnChange += OnReceiverValueChanged;
    }

    protected virtual void OnDisable()
    {
        if(variableReceiver == null) return;
        if(variableReceiver.Handler == null) return;
        variableReceiver.Handler.OnChange -= OnReceiverValueChanged;
    }

    protected virtual void OnReceiverValueChanged()
    {
        Debug.Log("currentVariableHandler.Handler.PreviousValue=" + currentVariableHandler.Handler.PreviousValue);
        Debug.Log("currentVariableHandler.Handler.CurrentValue=" + currentVariableHandler.Handler.CurrentValue);
    }

    public virtual void UpdateValue()
    {
        if(currentVariableHandler == null) return;
        if(currentVariableHandler.Handler == null) return;
        if(variableReceiver == null) return;
        if(variableReceiver.Handler == null) return;
        if(currentVariableHandler.Handler.CurrentValue == variableReceiver.Handler.CurrentValue)
        {
            return;
        }

        currentVariableHandler.SetValue(variableReceiver.Handler.CurrentValue);
    }
}
