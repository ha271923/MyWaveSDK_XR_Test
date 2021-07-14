#if UNITY_ANDROID && !UNITY_EDITOR
// #if UNITY_ANDROID 
#define ANDROID_DEVICE
#endif

using System;
using Htc.Viveport;
using UnityEngine;

public class AndroidFrameworkApi : MonoBehaviour
{
    private AndroidJavaObject mUnityActivity;
    private AndroidJavaObject mUnityApplicationContext;

    private AndroidJavaObject mAndroidApisObj;
    private AndroidJavaClass  mAndroidApisClz;

    private void Start()
    {
        mUnityActivity = UnityActivityHelper.GetActivity();
        mUnityApplicationContext = UnityActivityHelper.GetApplicationContext(mUnityActivity);
#if ANDROID_DEVICE
        if (mUnityActivity == null)
            Debug.LogError("AndroidFrameworkApi  Start()    mUnityActivity == null");

        // mAndroidApisClz = new AndroidJavaClass("com.htc.svr.androidSettingsLibrary.AndroidApis");
        mAndroidApisObj = new AndroidJavaObject("com.htc.svr.androidSettingsLibrary.AndroidApis");
        mAndroidApisObj.Call("setContext", mUnityActivity);
#else
        Debug.LogError("Not support this Script on non-Android device");
#endif
    }

    public void triggerVibration() {
        Debug.Log("triggerVibration !!!");
        Handheld.Vibrate();
    }

    public void adjustPlayArea(int areaSize) // normal key + priv-app
    {
#if ANDROID_DEVICE
        try
        {
            mAndroidApisObj.Call("adjustPlayArea", areaSize);
        }
        catch (Exception e)
        {
            Debug.LogError("AndroidFrameworkApi   adjustPlayArea     failed!");
        }
#else
        Debug.LogError("Not support this API on non-Android device");
#endif
    }

    public void enableDoubleTapPassThrough(bool enable) // normal key + priv-app + Fix AIDL duplicate issue
    {
#if ANDROID_DEVICE
        try
        {
            mAndroidApisObj.Call("enableDoubleTapPassThrough", enable);
        }
        catch (Exception e)
        {
            Debug.LogError("AndroidFrameworkApi   enableDoubleTapPassThrough    failed!");
        }
#else
        Debug.LogError("Not support this API on non-Android device");
#endif
    }

    public void enableCarMode(bool enable) // normal key + priv-app
    {
#if ANDROID_DEVICE
        try
        {
            mAndroidApisObj.Call("enableCarMode", enable);
        }
        catch (Exception e)
        {
            Debug.LogError("AndroidFrameworkApi   enableCarMode    failed!");
        }
#else
        Debug.LogError("Not support this API on non-Android device");
#endif
    }

    public void enableVirtualWall(bool enable) // normal key
    {
#if ANDROID_DEVICE
        try
        {
            mAndroidApisObj.Call("enableVirtualWall", enable);
        }
        catch (Exception e)
        {
            Debug.LogError("AndroidFrameworkApi   enableVirtualWall    failed!");
        }
#else
        Debug.LogError("Not support this API on non-Android device");
#endif
    }

    public void enterPhoneMode(bool enable) // normal key
    {
#if ANDROID_DEVICE
        try
        {
            mAndroidApisObj.Call("enterPhoneMode", enable);
        }
        catch (Exception e)
        {
            Debug.LogError("AndroidFrameworkApi   enterPhoneMode    failed!");
        }
#else
        Debug.LogError("Not support this API on non-Android device");
#endif
    }

    void DebugSrackInfo()
    {
        string trackStr = new System.Diagnostics.StackTrace().ToString();
        Debug.Log("Stack Info:" + trackStr);
    }
}
