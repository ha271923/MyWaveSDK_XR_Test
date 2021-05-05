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
using UnityEngine.UI;
using Wave.Essence;
using Wave.Native;

// public class IMEManagerClient : MonoBehaviour , IMECallbackIF
public class IMEManagerClient : MonoBehaviour
{
	private static string LOG_TAG = "IMEManagerClient";

	private InputField m_InputField;
	private string mInputContent = null;

	void Start()
	{

	}


	void LateUpdate()
	{

	}

    private void OnDestroy()
    {

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
}
