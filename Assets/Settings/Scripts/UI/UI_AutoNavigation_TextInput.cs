using HTC.UnityPlugin.CommonEventVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AutoNavigation_TextInput : UI_AutoNavigation
{
    [SerializeField] protected bool checkTextOnLimit = true;
    [SerializeField] protected CommonEventAsset TextRejectedEvent;
    [SerializeField] protected CommonEventAsset TextAcceptedEvent;
    [SerializeField] protected CommonEventAsset ForceCheckTextEvent;

    protected TMP_InputField CurrentInputField;
    private bool isChecked = false;
    private bool forceCheck = false;

    protected override bool CheckAcceptable() 
    {
        CurrentInputField = (TMP_InputField) CurrentUI;
        if(CurrentInputField == null) return false;
        return CheckTextAcceptable(CurrentInputField.text);
    }

    protected virtual bool CheckTextAcceptable(string text)
    {
        if(forceCheck)
        {
            forceCheck = false;
            bool contentAcceptable = CheckTextContentAcceptable(text);
            if (contentAcceptable && TextAcceptedEvent != null) TextAcceptedEvent.Trigger();
            else if (TextRejectedEvent != null) TextRejectedEvent.Trigger();
            return contentAcceptable;
        }

        if (checkTextOnLimit)
        {
            if (isChecked == false && text.Length == CurrentInputField.characterLimit)
            {
                isChecked = true;
                bool contentAcceptable = CheckTextContentAcceptable(text);
                if (contentAcceptable && TextAcceptedEvent != null) TextAcceptedEvent.Trigger();
                else if (TextRejectedEvent != null) TextRejectedEvent.Trigger();
                return contentAcceptable;
            }
            else if (CurrentInputField.text.Length < CurrentInputField.characterLimit)
            {
                isChecked = false;
            }
        }
        return false;
    }

    protected virtual bool CheckTextContentAcceptable(string text)
    {
        return false;
    }

    public virtual void OnSelect()
    {
        isStartAutoNavigation = true;
        if(ForceCheckTextEvent) ForceCheckTextEvent.Handler.OnTrigger += OnForceCheckText;
    }

    public virtual void OnDeselect()
    {
        isStartAutoNavigation = false;
        if(ForceCheckTextEvent) ForceCheckTextEvent.Handler.OnTrigger -= OnForceCheckText;
    }

    private void OnForceCheckText()
    {
        if (isStartAutoNavigation && navToNext == false)
        {
            Debug.Log("[AutoNavigation] Force check.");
            forceCheck = true;
        }
    }
}
