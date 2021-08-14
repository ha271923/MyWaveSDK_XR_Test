using HTC.UnityPlugin.CommonEventVariable;
using TMPro;
using UnityEngine;
using Wave.Essence;
using Wave.Native;

// for common event method
public struct VirtualKeyboardRequestSettings
{
    public VirtualKeyboardManager.KeyboardType keyboardType;
    public CommonEventHandler<string> recieverHandler;
    public CommonEventHandler doneHandler;
    public bool dynamicBroadcast;
    public bool broadcastWhenDone;
    public string existString;
}

public class VirtualKeyboardManager : MonoBehaviour
{
    public enum KeyboardType
    {
        General,
        Numeric,
        Password
    }

    private static string LOG_TAG = "vrsKeyboard";

    private static VirtualKeyboardManager instance = null;
    public static VirtualKeyboardManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<VirtualKeyboardManager>();
                if (instance == null)
                {
                    Debug.LogError("Need VirtualKeyboardManager in the scene.");
                }
            }

            return instance;
        }
    }

    public bool IsInitialized
    {
        get { return (IMEParameter != null && mEManager != null); }
    }

    public bool IsKeyboardShowed
    {
        get { return showKeyboard; }
    }

    public WVR_InputId inputButton = WVR_InputId.WVR_InputId_Alias1_Trigger;
    public Color pointerEnterColor = new Color(0.03529412f, 0.4862745f, 0.854902f, 0.5019608f);
    public Color pointerExitColor = Color.white;
    public Vector3 posFromHead = new Vector3(0.0f, -0.4f, -1.5f);
    public Vector3 rotFromOrigin = new Vector3(12.0f, 0.0f, 0.0f);
    private IMEManager mEManager = null;
    private IMEManager.IMEParameter IMEParameter = null;
    private string inputResult = "";
    private string existString = "";
    private TMP_InputField curInputTarget = null;
    private bool showKeyboard = false;
    private bool saveInputResult = false;
    private bool dynamicBroadcast = false;
    private bool broadcastWhenDone = true;
    private CommonEventHandler<string> broadcastHandler = null;
    private CommonEventHandler doneHandler = null;
    private KeyboardType keyboardType;

    private bool waitingDoneCB = false;
    public void InitIMEManager()
    {
        if (IMEManager.instance.isInitialized())
        {
            Log.d(LOG_TAG, "InitializeKeyboards: done");
            mEManager = IMEManager.instance;
        }
        else
        {
            Log.d(LOG_TAG, "InitializeKeyboards: failed");
            mEManager = null;
        }

        int MODE_FLAG_FIX_MOTION = 0x02;
        //int MODE_FLAG_AUTO_FIT_CAMERA = 0x04;
        int id = 0;
        int type = MODE_FLAG_FIX_MOTION;
        int mode = 2;

        string exist = "";
        int cursor = 0;
        int selectStart = 0;
        int selectEnd = 0;
        double[] pos = new double[] { posFromHead.x, posFromHead.y, posFromHead.z };
        Quaternion q = Quaternion.Euler(rotFromOrigin);
        double[] rot = new double[] { q.w, -1*q.x, q.y, q.z };
        int width = 800;
        int height = 800;
        int shadow = 100;
        string locale = "en_US";
        string title = "IMETest Title";
        int extraInt = 0;
        string extraString = "";
        int buttonId = (1 << (int)inputButton);
        IMEParameter = new IMEManager.IMEParameter(id, type, mode, exist, cursor, selectStart, selectEnd, pos,
                             rot, width, height, shadow, locale, title, extraInt, extraString, buttonId);
    }

    // state related
    private void ClearState()
    {
        existString = "";
        showKeyboard = false;
        dynamicBroadcast = false;
    }

    // input result related
    public void ClearInputResult()
    {
        inputResult = "";
        saveInputResult = false;
    }

    public void ClearUpdateInputField()
    {
        curInputTarget = null;
        broadcastWhenDone = true;
        broadcastHandler = null;
        doneHandler = null;
    }

    public void ShowKeyboard(VirtualKeyboardRequestSettings settings)
    {
        Log.i(LOG_TAG, $"showKeyboard click type: {settings.keyboardType}");
        if (!IsInitialized)
        {
            InitIMEManager();
        }

        //if (showKeyboard) HideKeyboard();   // needed?
        ClearInputResult();
        ClearUpdateInputField();
        waitingDoneCB = false;  // if former done cb was not trigger yet, but new keyboard is requested, set to false to prevent former string send to current requested.

        switch (settings.keyboardType)
        {
            case KeyboardType.General:
                IMEParameter.locale = "en_US";
                break;
            case KeyboardType.Numeric:
                IMEParameter.locale = "numeric";
                break;
            case KeyboardType.Password:
                IMEParameter.locale = "password";
                break;
        }

        // udpate state
        keyboardType = settings.keyboardType;
        broadcastHandler = settings.recieverHandler;
        doneHandler = settings.doneHandler;
        IMEParameter.exist = settings.existString;
        existString = settings.existString;
        dynamicBroadcast = settings.dynamicBroadcast;
        broadcastWhenDone = settings.broadcastWhenDone;
        IMEParameter.cursor = -1;

        // show keyboard
        IMEManager.instance.showKeyboard(IMEParameter, !dynamicBroadcast, InputDoneCallback, InputClickedCallBack);

        showKeyboard = true;
    }

    public void ShowKeyboard(KeyboardType type, TMP_InputField inputField)  // 0: default
    {
        Log.i(LOG_TAG, $"showKeyboard click type: {type}");
        if (!IsInitialized)
        {
            InitIMEManager();
        }

        //if (showKeyboard) HideKeyboard();   // needed?
        ClearInputResult();
        ClearUpdateInputField();
        waitingDoneCB = false;  // if former done cb was not trigger yet, but new keyboard is requested, set to false to prevent former string send to current requested.

        curInputTarget = inputField;

        switch (type)
        {
            case KeyboardType.General:
                IMEParameter.locale = "en_US";
                break;
            case KeyboardType.Numeric:
                IMEParameter.locale = "numeric";
                break;
            case KeyboardType.Password:
                IMEParameter.locale = "password";
                break;
        }

        keyboardType = type;
        IMEParameter.exist = curInputTarget.text;
        existString = curInputTarget.text; 
        IMEParameter.cursor = curInputTarget.caretPosition;

        TextMeshProUGUI placeholder = curInputTarget.placeholder.GetComponent<TextMeshProUGUI>();
        if (placeholder != null)
        {
            IMEParameter.title = placeholder.text;
        }
        else
        {
            IMEParameter.title = "";
        }

        IMEManager.instance.showKeyboard(IMEParameter, true, InputDoneCallback, InputClickedCallBack);

        showKeyboard = true;
    }

    private void CallIMEManagerHidekeyboard()
    {
        IMEManager.instance.hideKeyboard();
    }

    public void HideKeyboard()
    {
        if (IsInitialized && IsKeyboardShowed)
        {
            waitingDoneCB = true;
            ClearState();   // clear state but not clear input result, done cb will trigger after calling hidekeyboard.


            ////////// temp work around
            if (keyboardType == KeyboardType.General) Invoke("CallIMEManagerHidekeyboard", 0.05f);
            else CallIMEManagerHidekeyboard();
            //////////////////////////////////////////////////////////
        }
    }

    public void HideKeyboard(CommonEventHandler<string> reciever)  
    {
        if (IsKeyboardShowed && broadcastHandler == reciever)   // make sure the caller is same as the show caller.
        {
            Debug.Log("[IMEManager] Hide keyboard with common event handler");
            HideKeyboard();
        }
    }

    public void HideKeyboard(TMP_InputField reciever)   // make sure the caller is same as the show caller.
    {
        if (IsKeyboardShowed && reciever == curInputTarget)
        {
            Debug.Log("[IMEManager] Hide keyboard with TMP_InputField");
            HideKeyboard();
        }
    }

    public void InputDoneCallback(IMEManager.InputResult results)
    {
        //UnityMainThreadDispatcher.Instance().Enqueue(() =>
        //{
            if (waitingDoneCB)
            {
                Debug.Log("[IMEManager] done call back: " + results.InputContent.Length);
                if (saveInputResult)
                {
                    inputResult = results.InputContent;
                }
                else
                {
                    inputResult = existString;
                }

                if (broadcastWhenDone)
                {
                    Debug.Log("[IMEManager] broadcast when done");
                    BroadcastString(inputResult, true);
                }
                else
                {
                    ClearInputResult();
                    ClearUpdateInputField();
                }

                if (!dynamicBroadcast && doneHandler != null) doneHandler.Trigger();    // instant input will call done at clicked cb

                ClearState();
                //ClearInputResult();
                //ClearUpdateInputField();
                waitingDoneCB = false;
            }
        //});
    }

    public void InputClickedCallBack(IMEManager.InputResult results)
    {
        //UnityMainThreadDispatcher.Instance().Enqueue(() =>
        //{
            Debug.Log("[IMEManager][" + results.KeyCode + "] input call back");
            switch (results.KeyCode)
            {
                case IMEManager.InputResult.Key.NORMAL:
                    inputResult += results.InputContent;
                    if (dynamicBroadcast)
                    {
                        Debug.Log("[IMEManager] broadcast clicked instant");
                        BroadcastString(inputResult);

                    }
                    break;
                case IMEManager.InputResult.Key.ENTER:
                    saveInputResult = true;
                    if (dynamicBroadcast && doneHandler != null) doneHandler.Trigger();     // instant input call done here
                    HideKeyboard();
                    break;
                case IMEManager.InputResult.Key.CLOSE:      // never trigger ??? how to know user click close???
                    saveInputResult = false;
                    waitingDoneCB = true;
                    showKeyboard = false;
                    break;
                case IMEManager.InputResult.Key.BACKSPACE:
                    if (inputResult.Length > 0)
                    {
                        inputResult = inputResult.Remove(inputResult.Length - 1);
                    }
                    if (dynamicBroadcast)
                    {
                        Debug.Log("[IMEManager] broadcast clicked instant");
                        BroadcastString(inputResult);
                    }
                    break;
            }
        //});
    }

    private void BroadcastString(string value, bool clear = false)
    {
        if (broadcastHandler != null)
        {
            //Debug.Log("[IMEManager] broadcast with hanlder: " + broadcastHandler.Name);
            //broadcastHandler.Trigger(value);
            BroadcastString(value, broadcastHandler, clear) ;
        }
        else if (curInputTarget)
        {
            //Debug.Log("[IMEManager] update input field: " + curInputTarget.name);               
            //curInputTarget.text = value;
            BroadcastString(value, curInputTarget, clear);
        }
        else
        {               
            Debug.Log("[IMEManager] broadcast failed.");
        }
    }


    private void BroadcastString(string value, CommonEventHandler<string> eventBroadcastHandler, bool clear)
    {
        /*
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Debug.Log("[IMEManager] broadcast with hanlder");
            eventBroadcastHandler.Trigger(value);

            if(clear)
            {
                ClearInputResult();
                ClearUpdateInputField();
            }
        });
        */
    }

    private void BroadcastString(string value, TMP_InputField inputField, bool clear)
    {
        /*
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Debug.Log("[IMEManager] update input field");
            inputField.text = value;

            if (clear)
            {
                ClearInputResult();
                ClearUpdateInputField();
            }
        });
        */
    }
}
