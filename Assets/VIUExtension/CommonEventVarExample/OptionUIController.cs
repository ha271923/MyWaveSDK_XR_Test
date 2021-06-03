using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HTC.UnityPlugin.CommonEventVariable.Example
{
    public class OptionUIController : MonoBehaviour
    {
        public Toggle OptionToggleA;
        public Toggle OptionToggleB;
        public Toggle OptionToggleC;

        private CommonVariableHandler<SomeOption> someOptionVar = CommonVariable.Get<SomeOption>("OptionManager_OutSomeOptionVar");
        private CommonEventHandler<SomeOption> setSomeOptionEvent = CommonEvent.Get<SomeOption>("OptionManager_InSetSomeOption");

        private void OnEnable()
        {
            OnSomeOptionChanged();
            someOptionVar.OnChange += OnSomeOptionChanged;
            OptionToggleA.onValueChanged.AddListener(OnToggleAChanged);
            OptionToggleB.onValueChanged.AddListener(OnToggleBChanged);
            OptionToggleC.onValueChanged.AddListener(OnToggleCChanged);
        }

        private void OnToggleAChanged(bool value)
        {
            if (value) { setSomeOptionEvent.Trigger(SomeOption.OptionA); }
        }

        private void OnToggleBChanged(bool value)
        {
            if (value) { setSomeOptionEvent.Trigger(SomeOption.OptionB); }
        }

        private void OnToggleCChanged(bool value)
        {
            if (value) { setSomeOptionEvent.Trigger(SomeOption.OptionC); }
        }

        private void OnSomeOptionChanged()
        {
            switch (someOptionVar.CurrentValue)
            {
                default:
                case SomeOption.OptionA:
                    OptionToggleA.isOn = true;
                    break;
                case SomeOption.OptionB:
                    OptionToggleB.isOn = true;
                    break;
                case SomeOption.OptionC:
                    OptionToggleC.isOn = true;
                    break;
            }
        }

        private void OnDisable()
        {
            OptionToggleA.onValueChanged.RemoveListener(OnToggleAChanged);
            OptionToggleB.onValueChanged.RemoveListener(OnToggleBChanged);
            OptionToggleC.onValueChanged.RemoveListener(OnToggleCChanged);
            someOptionVar.OnChange -= OnSomeOptionChanged;
        }
    }
}