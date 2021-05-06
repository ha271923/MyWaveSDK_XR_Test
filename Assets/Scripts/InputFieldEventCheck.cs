using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;
using Wave.Native;

/**
 * Hook all TextMesh pro Events
 */
public class InputFieldEventCheck : MonoBehaviour
{
    private static string LOG_TAG = "InputFieldEventCheck";
    public TMP_InputField InputFieldComponent;

    void Awake()
    {
        Log.d(LOG_TAG, "Awake ++");
        InputFieldComponent = GetComponent<TMP_InputField>();

        // Events triggered when Input Field is selected.
        InputFieldComponent.onSelect.AddListener(OnSelect);
        InputFieldComponent.onDeselect.AddListener(OnDeselect);

        // Event triggered when Input Field text is changed.
        InputFieldComponent.onValueChanged.AddListener(OnValueChange);

        // Event triggered when pressing Enter or Return.
        InputFieldComponent.onSubmit.AddListener(OnSubmit);

        // Event triggered when text is no longer being edited.
        // This occurs OnDeselect, OnSubmit
        InputFieldComponent.onEndEdit.AddListener(OnEndEdit);

        // Events triggered when text is selected and unselected.
        InputFieldComponent.onTextSelection.AddListener(OnTextSelection);
        InputFieldComponent.onEndTextSelection.AddListener(OnEndTextSelection);

        InputFieldComponent.onValidateInput = OnValidateInput;
    }


    void OnDestroy()
    {
        Log.d(LOG_TAG, "OnDestroy --");
        InputFieldComponent.onSelect.RemoveAllListeners();
        InputFieldComponent.onDeselect.RemoveAllListeners();

        InputFieldComponent.onValueChanged.RemoveAllListeners();
        InputFieldComponent.onSubmit.RemoveAllListeners();
        InputFieldComponent.onEndEdit.RemoveAllListeners();

        InputFieldComponent.onTextSelection.RemoveAllListeners();
        InputFieldComponent.onEndTextSelection.RemoveAllListeners();
    }


    void OnValueChange(string text)
    {
        Log.d(LOG_TAG,"OnValueChange: New text=" + text + "    selectionAnchorPosition=" + InputFieldComponent.selectionAnchorPosition + "  selectionFocusPosition=" + InputFieldComponent.selectionFocusPosition);
        /*
        InputFieldComponent.text = string.Empty;

        for (int i = 0; i < text.Length; i++)
            InputFieldComponent.text += (int)text[i] + "-";
        */
    }

    void OnTextSelection(string text, int start, int end)
    {
        Log.d(LOG_TAG, "OnTextSelection:\nText has been selected. Range= " + start + " ~ " + end );
    }

    void OnEndTextSelection(string text, int start, int end)
    {
        Log.d(LOG_TAG,"OnEndTextSelection:");
    }

    void OnEndEdit(string text)
    {
        Log.d(LOG_TAG,"OnEndEdit: text = [" + text + "].");
        //Log.d(LOG_TAG,InputFieldComponent.selectionAnchorPosition + "  " + InputFieldComponent.selectionFocusPosition);
    }

    void OnSubmit(string text)
    {
        Log.d(LOG_TAG, "OnSubmit: text="+ text);
        InputFieldComponent.text = "OnSumbit: - [" + text + "]";
        //Log.d(LOG_TAG,InputFieldComponent.selectionAnchorPosition + "  " + InputFieldComponent.selectionFocusPosition);
    }

    public void OnSelect(string text)
    {
        Log.d(LOG_TAG, "OnSelect:\ntext=" + InputFieldComponent.text + " , length=" + InputFieldComponent.text.Length);
        //Log.d(LOG_TAG,InputField.caretPosition + "  " + InputField.selectionFocusPosition);
    }

    void OnDeselect(string text)
    {
        Log.d(LOG_TAG, "OnDeselect:\ntext=" + InputFieldComponent.text + " , length=" + InputFieldComponent.text.Length);
    }

    char OnValidateInput(string text, int charIndex, char addedChar)
    {
        Log.d(LOG_TAG, "OnValidateInput: " + text + "  " + addedChar);
        return addedChar;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Log.d(LOG_TAG, "OnPointerClick: eventData="+ eventData);
    }
}
