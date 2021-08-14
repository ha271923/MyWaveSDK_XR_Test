using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using HTC.UnityPlugin.CommonEventVariable;

public class VirtualKeyboardInputField : MonoBehaviour, IPointerClickHandler//, IDeselectHandler//, IPointerEnterHandler, IPointerExitHandler
{
    private static string LOG_TAG = "vrsKeyboard";

    [SerializeField] private CommonEventAsset RequestShowKeyboardEvent = null;
    [SerializeField] private CommonEventAsset RequestHideKeyboardEvent = null;
    [SerializeField] private CommonEventAssetString ReceiveInputStringEvent = null;     // required for getting cb string from common event
    [SerializeField] private CommonEventAsset KeyboardDoneEvent = null;
    [SerializeField] private CommonEventAsset RequestClearKeyboardInputEvent = null;

    [SerializeField] private bool showKeyboardOnPointerClicked = true;
    //[SerializeField] private bool hideKeyboardOnDeselect = true;
    [SerializeField] private bool clearStringWhenStartInput = true;
    [SerializeField] private bool dynamicUpdateInputField = false;      // instant input
    [SerializeField] private bool updateInputFieldWhenDone = true;      // if keyboard enter clicked, send result again (most case is for keyboard input panel shown)
    [SerializeField] private TMP_InputField inputField = null;      // required


    public void OnEnable()
    {
        if (inputField == null)
        {
            Debug.LogError("[SettingsUI][VirtualKeyboardInputField][" + gameObject.name + "] input field is null.");
        }

        if (ReceiveInputStringEvent == null)
        {
            Debug.Log("[SettingsUI][VirtualKeyboardInputField][" + gameObject.name + "]  ReceiveInputStringEvent is null, recommend using common event to handle input string from virtual keyboard.");
        }

        if (RequestShowKeyboardEvent)
        {
            RequestShowKeyboardEvent.Handler.OnTrigger += RequestShowKeyboard_OnTrigger;
        }

        if(RequestHideKeyboardEvent)
        {
            RequestHideKeyboardEvent.Handler.OnTrigger += RequestHidekeyboard_OnTrigger;
        }

        if(RequestClearKeyboardInputEvent)
        {
            RequestClearKeyboardInputEvent.Handler.OnTrigger += RequestClearKeyboardInput_OnTrigger;
        }

        if(KeyboardDoneEvent)
        {
            KeyboardDoneEvent.Handler.OnTrigger += KeyboardDone_OnTrigger;
        }
    }


    public void OnDisable()
    {
        if (RequestShowKeyboardEvent)
        {
            RequestShowKeyboardEvent.Handler.OnTrigger -= RequestShowKeyboard_OnTrigger;
        }

        if (RequestHideKeyboardEvent)
        {
            RequestHideKeyboardEvent.Handler.OnTrigger -= RequestHidekeyboard_OnTrigger;
        }

        if (RequestClearKeyboardInputEvent)
        {
            RequestClearKeyboardInputEvent.Handler.OnTrigger -= RequestClearKeyboardInput_OnTrigger;
        }

        if (KeyboardDoneEvent)
        {
            KeyboardDoneEvent.Handler.OnTrigger -= KeyboardDone_OnTrigger;
        }
    }

    private void RequestShowKeyboard_OnTrigger()
    {
        Debug.Log("[SettingsUI][VirtualKeyboard][" + inputField.name + "] Recieve show keyboard event.");
        ShowKeyBoard();
    }

    private void RequestHidekeyboard_OnTrigger()
    {
        if (VirtualKeyboardManager.Instance.IsKeyboardShowed)
        {
            Debug.Log("[SettingsUI][VirtualKeyboard][" + inputField.name + "] Recieve hide keyboard event.");
            HideKeyboard();
        }
    }

    // If the input string is not legal, you need to clear the virtual keyboard stored string if vitual keyboard keep showing (did not call hide and show)
    private void RequestClearKeyboardInput_OnTrigger()
    {
        if (VirtualKeyboardManager.Instance.IsKeyboardShowed)
        {
            VirtualKeyboardManager.Instance.ClearInputResult();
        }
    }

    // Most case is for instant input.
    virtual protected void KeyboardDone_OnTrigger()
    {
        Debug.Log("[SettingsUI][VirtualKeyboard][" + inputField.name + "] Recieve keyboard done");
        ReceiveInputStringEvent.Handler.OnTrigger -= OnRecieveInputString;
    }

    //    public void Awake()
    //    {
    //        inputField = GetComponent<TMP_InputField>();
    //#if UNITY_ANDROID && !UNITY_EDITOR
    //        // animator has no effect if set to false...
    //        //inputField.enabled = false;
    //#endif
    //    }

    public void OnPointerClick(PointerEventData data)
    {
        if (inputField.isFocused == false)
        {
            Debug.Log("[SettingsUI][VirtualKeyboard][" + inputField.name + "] is not slected with clicked, select again.");
            inputField.Select();    // make sure to trigger input field highlight (animator)
        }

        if (showKeyboardOnPointerClicked)
        {
            Debug.Log("[SettingsUI][VirtualKeyboard][" + inputField.name + "] show keyboard OnPointerClick");
            ShowKeyBoard();
        }
    }

    //public void OnPointerEnter(PointerEventData data)
    //{
    //    inputField.selectionColor = VirtualKeyboardManager.Instance.pointerEnterColor;
    //}

    //public void OnPointerExit(PointerEventData data)
    //{
    //    inputField.selectionColor = VirtualKeyboardManager.Instance.pointerExitColor;
    //}

    virtual protected void ShowKeyBoard()
    {
        if(inputField == null)
        {
            Debug.LogWarning("[SettingsUI][VirtualKeyboard] input field is null, show keyboard failed");
            return;
        }
       
        if (ReceiveInputStringEvent) Showkeyboard_CommonEvent();
        else ShowKeyboard_InputField();

        if (clearStringWhenStartInput)
        {
            inputField.text = "";
        }
    }

    // commmon evnet mehtod
    private void Showkeyboard_CommonEvent()
    {
        Debug.Log("[SettingsUI][VirtualKeyboard] show keyboard with common event");

        VirtualKeyboardRequestSettings settings;
        settings.recieverHandler = ReceiveInputStringEvent.Handler;
        if (KeyboardDoneEvent) settings.doneHandler = KeyboardDoneEvent.Handler;
        else settings.doneHandler = null;
        settings.dynamicBroadcast = dynamicUpdateInputField;
        settings.broadcastWhenDone = updateInputFieldWhenDone;
        settings.existString = inputField.text;
        switch (inputField.contentType)
        {
            case TMP_InputField.ContentType.Password:
                settings.keyboardType = VirtualKeyboardManager.KeyboardType.Password;
                break;
            case TMP_InputField.ContentType.DecimalNumber:
                settings.keyboardType = VirtualKeyboardManager.KeyboardType.Numeric;
                break;
            case TMP_InputField.ContentType.IntegerNumber:
                settings.keyboardType = VirtualKeyboardManager.KeyboardType.Numeric;
                break;
            default:
                settings.keyboardType = VirtualKeyboardManager.KeyboardType.General;
                break;
        }
        VirtualKeyboardManager.Instance.ShowKeyboard(settings);
        ReceiveInputStringEvent.Handler.OnTrigger += OnRecieveInputString;
    }

    // old method
    private void ShowKeyboard_InputField()
    {
        Debug.Log("[SettingsUI][VirtualKeyboard] show keyboard with input field");

        switch (inputField.contentType)
        {
            case TMP_InputField.ContentType.Password:
                VirtualKeyboardManager.Instance.ShowKeyboard(VirtualKeyboardManager.KeyboardType.General, inputField);
                break;
            case TMP_InputField.ContentType.DecimalNumber:
                VirtualKeyboardManager.Instance.ShowKeyboard(VirtualKeyboardManager.KeyboardType.Numeric, inputField);
                break;
            case TMP_InputField.ContentType.IntegerNumber:
                VirtualKeyboardManager.Instance.ShowKeyboard(VirtualKeyboardManager.KeyboardType.Numeric, inputField);
                break;
            default:
                VirtualKeyboardManager.Instance.ShowKeyboard(VirtualKeyboardManager.KeyboardType.General, inputField);
                break;
        }
    }

    private void HideKeyboard()
    {
        if (ReceiveInputStringEvent != null)
        {
            ReceiveInputStringEvent.Handler.OnTrigger -= OnRecieveInputString;
            VirtualKeyboardManager.Instance.HideKeyboard(ReceiveInputStringEvent.Handler);
        }
        else if (inputField != null) VirtualKeyboardManager.Instance.HideKeyboard(inputField);
    }

    // for common event recieve string cb, could inherit and override if needed.
    protected virtual void OnRecieveInputString(string value)
    {
        if(inputField == null)
        {
            Debug.Log("[SettingsUI][VirtualKeyboard][OnRecieveInputString] input filed is null.");
            return;
        }

        if(value == null)
        {
            Debug.Log("[SettingsUI][VirtualKeyboard][OnRecieveInputString] recieved string is null.");
            return;
        }

        Debug.Log("[SettingsUI][VirtualKeyboard] recieve input string");
        inputField.text = value;
    }
}
