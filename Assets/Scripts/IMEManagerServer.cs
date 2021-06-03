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
using Wave.Essence;
using Wave.Native;

namespace Hubble.Launcher.Input
{
	public class IMEManagerServer : MonoBehaviour
	{
		static public IMEManagerServer s_IMEManagerServer;
		private TMP_InputField m_InputField;
		private static string LOG_TAG = "IMEManagerServer";
		private IMEManagerWrapper2 mIMEWrapper;
		private string mInputContent = null;
		private StringBuilder onInputClickedSB;
		private string focusGOParentGOname;

		private bool mIsShowKeyboardInputPanel = true;
		void Start()
		{
			s_IMEManagerServer = gameObject.GetComponent<IMEManagerServer>();
			mIMEWrapper = IMEManagerWrapper2.GetInstance();
		}

		public void setCallbacks(GameObject GO)
		{
			IMEManagerClient inputClient;
			inputClient = GetIMEClient(GO);

			if (inputClient != null)
			{
				Log.d(LOG_TAG, "setCallbacks");
				// IMECallbackIF inputClickCallback = (IMECallbackIF)m_InputFocusGameObject;
				mIMEWrapper.SetDoneCallback(InputDoneCallbackImpl); // Bug: WaveSDK might not support .SetText() .SetTitle() if IME panel enabled.
				// mIMEWrapper.SetClickedCallback(InputClickCallbackImpl);
			}
			else
			{
				Debug.LogError("inputClient == null ");
			}
		}

		private TMP_InputField GetInputField(GameObject GO)
		{
			Log.d(LOG_TAG, "GetInputField() +++");
			if (GO != null)
			{
				TMP_InputField inputObj = GO.GetComponent<TMP_InputField>();
				Log.d(LOG_TAG, "this=" + this);
				Log.d(LOG_TAG, "GO=" + GO);
				Log.d(LOG_TAG, "GO.transform.name:" + GO.transform.name);
				return inputObj;
			}
			else
			{
				Debug.LogError("GO == null");
				throw new System.NullReferenceException();
			}
			Log.d(LOG_TAG, "GetInputField() ---");
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


		public void showKeyboard(GameObject GO)
		{
			Log.i(LOG_TAG, "ShowKeyboard");

			if (GO == null)
			{
				Log.e(LOG_TAG, "ShowKeyboard(null) from null GO");
				new System.NullReferenceException();
			}
			m_InputField = GetInputField(GO);
			focusGOParentGOname = Utils.GameObjectUtils.getParentGOname(GO);
			Log.i(LOG_TAG, "focusGOParentGOname = " + focusGOParentGOname);
			if (m_InputField != null)
			{
				m_InputField.shouldHideMobileInput = true;
				setCallbacks(GO);
				Log.i(LOG_TAG, "InputField.text = " + m_InputField.text);
				onInputClickedSB = new StringBuilder(m_InputField.text);
				mIMEWrapper.SetText(m_InputField.text); // Bug: WaveSDK might not support
			}
			else
			{
				Log.e(LOG_TAG, "s_InputGameObject=" + GO + " m_InputField == " + m_InputField);
			}
			mIMEWrapper.SetTitle("Input..."); // Bug: WaveSDK might not support
			mIMEWrapper.SetLocale(IMEManagerWrapper2.Locale.en_US);
			mIMEWrapper.SetAction(IMEManagerWrapper2.Action.Enter);
			mIMEWrapper.Show(mIsShowKeyboardInputPanel);
		}

		public void hideKeyboard()
		{
			mIMEWrapper.Hide();
			onInputClickedSB = null;
		}

		public void InputDoneCallbackImpl(IMEManagerWrapper2.InputResult results)
		{
			Log.d(LOG_TAG, "inputDoneCallbackImpl: " + results.GetContent());
			mInputContent = results.GetContent();
		}
		public void InputClickCallbackImpl(IMEManagerWrapper2.InputResult results)
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
						mInputContent = (onInputClickedSB.Length - 1).ToString();
					}
					else if (onInputClickedSB.Length == 1)
					{
						mInputContent = "";
					}

				}
				else if (results.GetKeyCode() == IMEManager.InputResult.Key.ENTER)
				{
					Log.d(LOG_TAG, "on clicked ENTER key");
					hideKeyboard();
				}
				else if (results.GetKeyCode() == IMEManager.InputResult.Key.CLOSE)
				{
					Log.d(LOG_TAG, "on clicked CLOSE key");
					hideKeyboard();
				}
				else
				{
					onInputClickedSB.Append(results.GetContent());
					mInputContent = onInputClickedSB.ToString();
					Log.d(LOG_TAG, "InputClickCallbackImpl   mInputContent=" + mInputContent + "  m_InputField=" + m_InputField + "  m_InputField.text=" + m_InputField.text + "  m_InputField.textComponent.text=" + m_InputField.textComponent.text);
				}
			}
			else
			{
				Log.e(LOG_TAG, "m_InputField == null !!");
			}
		}

		private void UpdateInputField(string str)
		{
			Log.d(LOG_TAG, "XX UpdateInputField()   m_InputField=" + m_InputField + "  str=" + str);
			if (m_InputField != null && str != null)
			{
				m_InputField.text = str;
			}
			else
			{
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
						Log.d(LOG_TAG, "found child GO=" + childGO);
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

		public int getKeyboardStatus()
		{
			int state = mIMEWrapper.getKeyboardState();
			Log.d(LOG_TAG, "getKeyboardState()= " + state);
			return state;
		}
	}
}