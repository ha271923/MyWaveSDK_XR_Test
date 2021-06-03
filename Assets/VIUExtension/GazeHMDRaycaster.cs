//========= Copyright 2016-2020, HTC Corporation. All rights reserved. ===========

#pragma warning disable 0649
using UnityEngine;
using UnityEngine.EventSystems;
using Wave.Native;

namespace HTC.UnityPlugin.Pointer3D
{
    public class GazeHMDRaycaster : Pointer3DRaycaster
    {
        [SerializeField]
        private bool mouseButtonLeftPressed = false;
        [SerializeField]
        private bool mouseButtonRightPressed = false;
        [SerializeField]
        private bool mouseButtonMiddlePressed = false;

        public bool mouseButtonLeft { get { return mouseButtonLeftPressed; } set { mouseButtonLeftPressed = value; } }
        public bool mouseButtonMiddle { get { return mouseButtonRightPressed; } set { mouseButtonRightPressed = value; } }
        public bool mouseButtonRight { get { return mouseButtonMiddlePressed; } set { mouseButtonMiddlePressed = value; } }

        private bool currentHMDButtonPressed = false;
        private bool lastHMDButtonPressed = false;

        public bool CurrentHMDButtonPressed { get { return currentHMDButtonPressed; } }
        public bool LastHMDButtonPressed { get { return lastHMDButtonPressed; } }


        public bool GetButtonTriggerState(PointerEventData.InputButton btn)
        {
            switch (btn)
            {
                default:
                case PointerEventData.InputButton.Left: return mouseButtonLeftPressed;
                case PointerEventData.InputButton.Right: return mouseButtonRightPressed;
                case PointerEventData.InputButton.Middle: return mouseButtonMiddlePressed;
            }
        }

        protected override void Start()
        {
            base.Start();
            buttonEventDataList.Add(new GazeHMDEventData(this, EventSystem.current, PointerEventData.InputButton.Left));
            buttonEventDataList.Add(new GazeHMDEventData(this, EventSystem.current, PointerEventData.InputButton.Right));
            buttonEventDataList.Add(new GazeHMDEventData(this, EventSystem.current, PointerEventData.InputButton.Middle));
        }

        public override void Raycast()
        {
            base.Raycast();

            lastHMDButtonPressed = currentHMDButtonPressed;
#if UNITY_ANDROID && !UNITY_EDITOR
            currentHMDButtonPressed = Interop.WVR_GetInputButtonState(WVR_DeviceType.WVR_DeviceType_HMD, WVR_InputId.WVR_InputId_Alias1_Enter);
#endif

#if UNITY_EDITOR
            currentHMDButtonPressed = Input.GetKey(KeyCode.Space);
#endif
        }
    }

    public class GazeHMDEventData : Pointer3DEventData
    {
        public GazeHMDRaycaster pointerRaycaster { get; private set; }

        public GazeHMDEventData(GazeHMDRaycaster ownerRaycaster, EventSystem eventSystem, InputButton btn) : base(ownerRaycaster, eventSystem)
        {
            pointerRaycaster = ownerRaycaster;
            button = btn;
        }

        public override bool GetPress()
        {
            return pointerRaycaster.CurrentHMDButtonPressed;
        }

        public override bool GetPressDown()
        {
            return !pointerRaycaster.LastHMDButtonPressed && pointerRaycaster.CurrentHMDButtonPressed;
        }

        public override bool GetPressUp()
        {
            return pointerRaycaster.LastHMDButtonPressed && !pointerRaycaster.CurrentHMDButtonPressed;
        }
    }
}