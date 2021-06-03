//========= Copyright 2016-2020, HTC Corporation. All rights reserved. ===========

#pragma warning disable 0649
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HTC.UnityPlugin.Pointer3D
{
    public class GazePointerRaycaster : Pointer3DRaycaster
    {
        [SerializeField]
        private float mouseButtonLeftDuration = 2f;
        [SerializeField]
        private float mouseButtonRightDuration = 0f;
        [SerializeField]
        private float mouseButtonMiddleDuration = 0f;
        [SerializeField]
        private float clickDuration = 0.05f;

        public float MouseButtonLeftDuration { get { return mouseButtonLeftDuration; } set { mouseButtonLeftDuration = value; } }
        public float MouseButtonRightDuration { get { return mouseButtonRightDuration; } set { mouseButtonRightDuration = value; } }
        public float MouseButtonMiddleDuration { get { return mouseButtonMiddleDuration; } set { mouseButtonMiddleDuration = value; } }

        private Selectable lastFrameHovered;
        private Selectable currentFrameHovered;
        private float hoverTime;

        public Selectable LastFrameHovered { get { return lastFrameHovered; } }
        public Selectable CurrentFrameHovered { get { return currentFrameHovered; } }
        public float HoveredTime { get { return hoverTime; } }
        public float ClickDuration { get { return clickDuration; } }

        public float GetButtonTriggerDuration(PointerEventData.InputButton btn)
        {
            switch (btn)
            {
                default:
                case PointerEventData.InputButton.Left: return mouseButtonLeftDuration;
                case PointerEventData.InputButton.Right: return mouseButtonRightDuration;
                case PointerEventData.InputButton.Middle: return mouseButtonMiddleDuration;
            }
        }

        protected override void Start()
        {
            base.Start();
            buttonEventDataList.Add(new GazePointerEventData(this, EventSystem.current, PointerEventData.InputButton.Left));
            buttonEventDataList.Add(new GazePointerEventData(this, EventSystem.current, PointerEventData.InputButton.Right));
            buttonEventDataList.Add(new GazePointerEventData(this, EventSystem.current, PointerEventData.InputButton.Middle));
        }

        public override void Raycast()
        {
            base.Raycast();

            lastFrameHovered = currentFrameHovered;

            var hitResult = FirstRaycastResult();
            currentFrameHovered = hitResult.isValid ? hitResult.gameObject.GetComponentInParent<Selectable>() : null;
            if (currentFrameHovered != lastFrameHovered)
            {
                hoverTime = Time.time;
            }
        }
    }

    public class GazePointerEventData : Pointer3DEventData
    {
        public GazePointerRaycaster pointerRaycaster { get; private set; }

        public GazePointerEventData(GazePointerRaycaster ownerRaycaster, EventSystem eventSystem, InputButton btn) : base(ownerRaycaster, eventSystem)
        {
            pointerRaycaster = ownerRaycaster;
            button = btn;
        }

        private bool IsPressed(Selectable hovered, float hoveredDuration, float triggerDuration)
        {
            if (hovered == null) { return false; }
            return hoveredDuration >= triggerDuration && hoveredDuration < (triggerDuration + pointerRaycaster.ClickDuration);
        }

        public override bool GetPress()
        {
            return IsPressed(pointerRaycaster.CurrentFrameHovered, Time.time - pointerRaycaster.HoveredTime, pointerRaycaster.GetButtonTriggerDuration(button));
        }

        public override bool GetPressDown()
        {
            var duration = Time.time - pointerRaycaster.HoveredTime;
            var triggerDuration = pointerRaycaster.GetButtonTriggerDuration(button);
            return
                !IsPressed(pointerRaycaster.LastFrameHovered, duration - Time.deltaTime, triggerDuration) &&
                IsPressed(pointerRaycaster.CurrentFrameHovered, duration, triggerDuration);
        }

        public override bool GetPressUp()
        {
            var duration = Time.time - pointerRaycaster.HoveredTime;
            var triggerDuration = pointerRaycaster.GetButtonTriggerDuration(button);
            return
                IsPressed(pointerRaycaster.LastFrameHovered, duration - Time.deltaTime, triggerDuration) &&
                !IsPressed(pointerRaycaster.CurrentFrameHovered, duration, triggerDuration);
        }
    }
}