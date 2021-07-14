using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Htc.Viveport
{
    public static class UnityActivityHelper
    {
        private const string UNITY_PLAYER_CLASS = "com.unity3d.player.UnityPlayer";

        //#if UNITY_ANDROID && !UNITY_EDITOR
        /// Returns the Android Activity used by the Unity device player. The caller is
        /// responsible for memory-managing the returned AndroidJavaObject.
        public static AndroidJavaObject GetActivity()
        {
            AndroidJavaClass jc = new AndroidJavaClass(UNITY_PLAYER_CLASS);
            if (jc == null)
            {
                Debug.LogErrorFormat("Failed to get class {0}", UNITY_PLAYER_CLASS);
                return null;
            }
            AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity == null)
            {
                Debug.LogError("Failed to obtain current Android activity.");
                return null;
            }
            return activity;
        }

        /// Returns the application context of the current Android Activity.
        public static AndroidJavaObject GetApplicationContext(AndroidJavaObject activity)
        {
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
            if (context == null)
            {
                Debug.LogError("Failed to get application context from Activity.");
                return null;
            }
            return context;
        }
    }
}