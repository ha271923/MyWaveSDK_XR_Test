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
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wave.Essence;
using Wave.Native;

public class IMEManagerServer : MonoBehaviour
{
	static private GameObject s_focusGO;
	private TMP_InputField m_InputField;
	private static string LOG_TAG = "IMEManagerServer";
	private IMEManagerWrapper mIMEWrapper;
	private string mInputContent = null;
	private StringBuilder onInputClickedSB;

	private bool mIsShowKeyboardInputPanel = false;
	void Start()
	{
		mIMEWrapper = IMEManagerWrapper.GetInstance();
	}


	public void setCallbacks(GameObject GO) {
		IMEManagerClient inputClient;
		inputClient = GetIMEClient(GO);

		if (inputClient != null)
		{
			Debug.Log("setCallbacks");
			// IMECallbackIF inputClickCallback = (IMECallbackIF)m_InputFocusGameObject;
			// mIMEWrapper.SetDoneCallback(InputDoneCallbackImpl);
			mIMEWrapper.SetClickedCallback(InputClickCallbackImpl);
		}
		else {
			Debug.LogError("inputClient == null ");
		}
	}

	private TMP_InputField GetInputField()
	{
		Log.d(LOG_TAG, "GetInputField() +++");
		if (s_focusGO != null)
		{
			TMP_InputField inputObj = s_focusGO.GetComponent<TMP_InputField>();
			Log.d(LOG_TAG, "this=" + this);
			Log.d(LOG_TAG, "s_InputGameObject=" + s_focusGO);
			Log.d(LOG_TAG, "s_InputGameObject.transform.name:" + s_focusGO.transform.name);
			return inputObj;
		}
		else
		{
			Debug.LogError("m_InputGameObject == null");
			throw new System.NullReferenceException();
		}
		return null;
	}

	private TMP_InputField GetInputField(string name)
	{
		TMP_InputField inputObj = GameObject.Find(name).GetComponent<TMP_InputField>();
		return inputObj;
	}

	private IMEManagerClient GetIMEClient(GameObject GO)
	{
		Log.d(LOG_TAG, "GetIMEScript() +++");
		IMEManagerClient inputClient = GO.GetComponent<IMEManagerClient>();
		Log.d(LOG_TAG, "this=" + this);
		Log.d(LOG_TAG, "GO=" + GO);
		Log.d(LOG_TAG, "GO.transform.name:" + GO.transform.name);
		return inputClient;
	}


	public void ShowKeyboard(GameObject GO)
	{
		Log.i(LOG_TAG, "ShowKeyboard");

		s_focusGO = GO;
		if (GO == null)
		{
			Log.e(LOG_TAG, "ShowKeyboard(null) from null GO");
			new System.NullReferenceException();
		}
		m_InputField = GetInputField();

		if (m_InputField != null)
		{
			m_InputField.shouldHideMobileInput = true;
			setCallbacks(s_focusGO);
			Log.i(LOG_TAG, "InputField.text = " + m_InputField.text);
			onInputClickedSB = new StringBuilder(m_InputField.text);
			mIMEWrapper.SetText(m_InputField.text);
		}
		else
		{
			Log.e(LOG_TAG, "s_InputGameObject="+s_focusGO+" m_InputField == "+m_InputField);
		}
		mIMEWrapper.SetTitle("Input...");
		mIMEWrapper.SetLocale(IMEManagerWrapper.Locale.en_US);
		mIMEWrapper.SetAction(IMEManagerWrapper.Action.Enter);
		mIMEWrapper.Show(mIsShowKeyboardInputPanel);
	}

	public void HideKeyboard()
	{
		mIMEWrapper.Hide();
		s_focusGO = null;
		onInputClickedSB = null;
	}



	public void InputDoneCallbackImpl(IMEManagerWrapper.InputResult results)
	{
		Log.d(LOG_TAG, "inputDoneCallbackImpl: " + results.GetContent());
		onInputClickedSB.Append(results.GetContent());
		mInputContent = onInputClickedSB.ToString();
	}
	public void InputClickCallbackImpl(IMEManagerWrapper.InputResult results)
	{
		Log.d(LOG_TAG, "InputClickCallbackImpl:  clickedKeyChar=" + results.GetContent());

		// Note: directly update input field text in UI thread will exception
		// use LastUpdate to update Input field text
		if (onInputClickedSB != null)
		{
			if (results.GetKeyCode() == IMEManager.InputResult.Key.BACKSPACE)
			{
				Log.d(LOG_TAG, "on clicked BACKSPACE key");
				if (onInputClickedSB.Length > 1)
				{
					mInputContent = (onInputClickedSB.Length -1).ToString();
				}
				else if (onInputClickedSB.Length == 1)
				{
					mInputContent = "";
				}

			}
			else if (results.GetKeyCode() == IMEManager.InputResult.Key.ENTER)
			{
				Log.d(LOG_TAG, "on clicked ENTER key");
				HideKeyboard();
			}
			else if (results.GetKeyCode() == IMEManager.InputResult.Key.CLOSE)
			{
				Log.d(LOG_TAG, "on clicked CLOSE key");
				HideKeyboard();
			}
			else
			{
				onInputClickedSB.Append(results.GetContent());
				mInputContent = onInputClickedSB.ToString();
				Log.d(LOG_TAG, "InputClickCallbackImpl   mInputContent=" + mInputContent + "  m_InputField=" + m_InputField + "  m_InputField.text="+ m_InputField.text + "  m_InputField.textComponent.text=" + m_InputField.textComponent.text );
			}
		}
		else {
			Log.e(LOG_TAG, "m_InputField == null !!");
		}
	}

	private void UpdateInputField(string str)
	{
		Log.d(LOG_TAG, "XX UpdateInputField()   m_InputField=" + m_InputField + "  str=" + str);
		if (m_InputField != null && str != null)
		{

			// m_InputField.textComponent.text = str;
			m_InputField.text = str;
			/*
			var childCount = s_focusGO.transform.childCount;
			if (childCount != 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					if (s_focusGO.transform.GetChild(i).gameObject.transform.name == "Text_Field")
					{
						Log.d(LOG_TAG, "found Text_Field GO");
						GameObject childGO = s_focusGO.transform.GetChild(i).gameObject;
						if (childGO.transform.GetChild(i).gameObject.transform.name == "Placeholder")
						{
							Log.d(LOG_TAG, "found Placeholder GO");
							GameObject placeholderGO = childGO.transform.GetChild(i).gameObject;
							placeholderGO.GetComponent<TMP_Text>().SetText("");
						}
						if (childGO.transform.GetChild(i).gameObject.transform.name == "Text_Input")
						{
							Log.d(LOG_TAG, "found Text_Input GO");
							GameObject text_inputGO = childGO.transform.GetChild(i).gameObject;
							text_inputGO.GetComponent<TMP_Text>().SetText(str);
						}

					}
				}
			}
			*/
		}
		else {
			Log.d(LOG_TAG, "m_InputField=" + m_InputField + "   str=" + str);
		}
	}

	static public bool findChildGO(GameObject currentGO, string childGO)
	{
		var childCount = currentGO.transform.childCount;
		if (childCount != 0)
			for (int i = 0; i < childCount; i++)
				if (currentGO.transform.GetChild(i).gameObject.transform.name == childGO)
				{
					Debug.Log("found child GO=" + childGO);
					return true;
				}

		return false;
	}


	void LateUpdate()
	{
		if (mInputContent != null)
		{
			UpdateInputField(mInputContent);

			mInputContent = null;
		}
	}

}
