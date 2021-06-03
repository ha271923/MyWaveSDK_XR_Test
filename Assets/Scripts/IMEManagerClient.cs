// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Wave.Essence;
using Wave.Native;
using TMPro;

namespace Hubble.Launcher.Input
{
    // public class IMEManagerClient : MonoBehaviour , IMECallbackIF
    public class IMEManagerClient : MonoBehaviour, IPointerClickHandler
{
	private static string LOG_TAG = "IMEManagerClient";
        private string focusGOParentGOname;

    private TMP_InputField InputFieldComponent;

    private int MAX_INPUT_CHAR = 3;

    void Start()
	{
            showDebugLog();


        focusGOParentGOname = Utils.GameObjectUtils.getParentGOname(gameObject);
        InputFieldComponent = gameObject.GetComponent<TMP_InputField>();
        InputFieldComponent = GetComponent<TMP_InputField>();

        // ========== addListener() by Inspector UI or following code ==========
        InputFieldComponent.onValueChanged.AddListener(OnValueChange);
        InputFieldComponent.onEndEdit.AddListener(OnEndEdit); // This occurs OnDeselect, OnSubmit when text is no longer being edited.
        InputFieldComponent.onSelect.AddListener(OnSelect); // onSelect == onFocus
        InputFieldComponent.onDeselect.AddListener(OnDeselect);  // onDeselect == onFocusLost

        // ========== addListener() by following code only ==========
        InputFieldComponent.onSubmit.AddListener(OnSubmit); // pressing Enter or Return.
        // Events triggered when text is selected and unselected.
        InputFieldComponent.onTextSelection.AddListener(OnTextSelection);
        InputFieldComponent.onEndTextSelection.AddListener(OnEndTextSelection);


        InputFieldComponent.onValidateInput = OnValidateInput;
        // Sets the MyValidate method to invoke after the input field's default input validation invoke (default validation happens every time a character is entered into the text field.)
        /*
            InputFieldComponent.onValidateInput += delegate (string input, int charIndex, char addedChar) {
            Log.d(LOG_TAG, ".onValidateInput += delegate(...)");
            return MyValidate(addedChar); 
        };
        */
    }


    void LateUpdate()
	{

	}

    void OnDestroy()
    {
        Log.d(LOG_TAG, "OnDestroy --");
        InputFieldComponent.onValueChanged.RemoveAllListeners();
        InputFieldComponent.onEndEdit.RemoveAllListeners();
        InputFieldComponent.onSelect.RemoveAllListeners();
        InputFieldComponent.onDeselect.RemoveAllListeners();

        InputFieldComponent.onSubmit.RemoveAllListeners();
        InputFieldComponent.onTextSelection.RemoveAllListeners();
        InputFieldComponent.onEndTextSelection.RemoveAllListeners();
    }

    private const int INPUTFIELD_CHAR_MAX = 99;
    // ================= Show in TMP InputField Inspector UI =========================
    void OnValueChange(string text)
    {
        Log.d(LOG_TAG, "OnValueChange:\n New text = [" + text + "],  selectionAnchorPosition=" + InputFieldComponent.selectionAnchorPosition + "  selectionFocusPosition=" + InputFieldComponent.selectionFocusPosition);
        if ( text.Length > INPUTFIELD_CHAR_MAX ) {
                Log.d(LOG_TAG, "OnValueChange:\n text.Length=" + text.Length + " is large than " + INPUTFIELD_CHAR_MAX +" !!!!!!!!!!!!!!!!!!!!");
                InputFieldComponent.text = text.Substring(0, text.Length - 1);
        }
    }

    void OnEndEdit(string text)
    {
        Log.d(LOG_TAG, "OnEndEdit:\n New text = [" + text + "],  selectionAnchorPosition=" + InputFieldComponent.selectionAnchorPosition + "  selectionFocusPosition=" + InputFieldComponent.selectionFocusPosition);
    }

    public void OnSelect(string text)
    {
        Log.d(LOG_TAG, "OnSelect:\n text = [" + InputFieldComponent.text + "] , length=" + InputFieldComponent.text.Length + "  caretPosition=" + InputFieldComponent.caretPosition + "  selectionFocusPosition=" + InputFieldComponent.selectionFocusPosition);
        IMEManagerServer.s_IMEManagerServer.showKeyboard(gameObject);
    }

    void OnDeselect(string text)
    {
        Log.d(LOG_TAG, "OnDeselect:\n text = [" + InputFieldComponent.text + "] , length=" + InputFieldComponent.text.Length);
        IMEManagerServer.s_IMEManagerServer.hideKeyboard();
    }

    // ================= Hide in TMP InputField Inspector UI =========================
    void OnTextSelection(string text, int start, int end)
    {
        Log.d(LOG_TAG, "OnTextSelection:\nText has been selected. Range= " + start + " ~ " + end);
    }

    void OnEndTextSelection(string text, int start, int end)
    {
        Log.d(LOG_TAG, "OnEndTextSelection:\nText has been selected. Range= " + start + " ~ " + end);
    }

    void OnSubmit(string text)
    {
        Log.d(LOG_TAG, "OnSubmit:\n text = [" + text+ "]");
        InputFieldComponent.text = text;
        //Log.d(LOG_TAG,InputFieldComponent.selectionAnchorPosition + "  " + InputFieldComponent.selectionFocusPosition);
    }

    char OnValidateInput(string text, int charIndex, char addedChar)
    {
        Log.d(LOG_TAG, "OnValidateInput:\n text = [" + text + "] ,  charIndex=" + charIndex + "  addedChar= " + addedChar);
        return addedChar;
    }

    private char MyValidate(char charToValidate)
    {
        Log.d(LOG_TAG, "OnValidateInput>MyValidate:\n charToValidate = [" + charToValidate + "]");

        //Checks if a dollar sign is entered....
        if (charToValidate == '$')
        {
            // ... if it is change it to an empty character.
            charToValidate = '\0';
        }
        return charToValidate;
    }

        // ------------------------------ implementing IPointerClickHandler I/F in Script ------------------------------
        public void OnPointerClick(PointerEventData eventData)  // addListener() by IPointerClickHandler interface implementation in Script
    {
            // Log.d(LOG_TAG, "OnPointerClick:\n eventData=" + eventData);
        Log.d(LOG_TAG, "OnPointerClick:");
        int status = IMEManagerServer.s_IMEManagerServer.getKeyboardStatus();
        IMEManagerServer.s_IMEManagerServer.showKeyboard(gameObject);
    }

        /*
        public void InputDoneCallbackImpl(IMEManagerWrapper.InputResult results)
        {
            throw new System.NotImplementedException();
        }

        public void InputClickCallbackImpl(IMEManagerWrapper.InputResult results)
        {
            throw new System.NotImplementedException();
        }
        */

        private void showDebugLog()
        {
            Log.i(LOG_TAG, "showDebugLog Log.i --------------------");
            Debug.Log("showDebugLog --------------------");
            Debug.LogWarning("showDebugLogWarn --------------------");
            Debug.LogError("showDebugLogError --------------------");

        }

    }


}