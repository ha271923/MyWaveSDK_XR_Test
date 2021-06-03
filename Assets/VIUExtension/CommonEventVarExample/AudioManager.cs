using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HTC.UnityPlugin.CommonEventVariable.Example
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip buttonHover;
        public AudioClip buttonClick;

        private CommonEventHandler standardButtonHoverEvent = CommonEvent.Get("AudioStandardButton_Hover");
        private CommonEventHandler standardButtonClickEvent = CommonEvent.Get("AudioStandardButton_Click");

        private void Awake()
        {
            standardButtonHoverEvent.OnTrigger += () => PlayClip("buttonHover", buttonHover);
            standardButtonClickEvent.OnTrigger += () => PlayClip("buttonClick", buttonClick);
        }

        private void PlayClip(string clipName, AudioClip clip)
        {
            Debug.Log("[AudioManager] play clip: " + clipName);
            // mix some audio clips here...
        }
    }
}