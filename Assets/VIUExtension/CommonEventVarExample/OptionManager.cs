using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTC.UnityPlugin.CommonEventVariable.Example
{
    public class OptionManager : MonoBehaviour
    {
        public SomeOption defaultOption = SomeOption.OptionB;
        private CommonVariableHandler<SomeOption> someOptionVar = CommonVariable.Get<SomeOption>("OptionManager_OutSomeOptionVar");
        private CommonEventHandler<SomeOption> setSomeOptionEvent = CommonEvent.Get<SomeOption>("OptionManager_InSetSomeOption");
        IEnumerable<SomeOption> vars = (IEnumerable<SomeOption>)CommonVariable.AllVariables();

        private void Awake()
        {
            someOptionVar.SetValue(defaultOption);

            setSomeOptionEvent.OnTrigger += OnSetSomeOption;
        }

        private void OnDestroy()
        {
            setSomeOptionEvent.OnTrigger -= OnSetSomeOption;
        }

        private void OnSetSomeOption(SomeOption value)
        {
            //// Send value back to system
            Debug.Log("[OptionManager] OnSetSomeOptionEvent(" + value + ") received");

            //// "SetValue" will set someOptionVar.CurrentValue to the desired value
            //// It also uses default equality function to compare CurrentValue & PreviousValue
            //// Will emit someOptionVar.OnChange event synchronously if valus are not equal
            someOptionVar.SetValue(value);

            //// It is possible to manually control OnChange event triggered time by using SetValueWithoutNotify & NotifyAndResetIfChanged
            //// SetValueWithoutNotify will force set IsValueChanged flag so NotifyAndResetIfChanged will trigger OnChange event
            //if (someOptionVar.CurrentValue != value)
            //{
            //    someOptionVar.SetValueWithoutNotify(value);
            //    // do some other things...
            //    someOptionVar.NotifyAndResetIfChanged();
            //}
        }

        private void Update()
        {
            //// Pull value from system
            if (Input.GetKeyDown(KeyCode.A))
            {
                OnSetSomeOption(SomeOption.OptionA);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                OnSetSomeOption(SomeOption.OptionB);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                OnSetSomeOption(SomeOption.OptionC);
            }
        }
    }
}