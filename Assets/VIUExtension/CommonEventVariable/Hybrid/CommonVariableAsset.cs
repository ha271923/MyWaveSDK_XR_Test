#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    public abstract class CommonVariableAssetBase : CommonHandlerAsset
    {
        protected sealed override CommonHandler HandlerBase { get { return VariableHandlerBase; } }
        public abstract CommonVariableHandlerBase VariableHandlerBase { get; }

        public int SetValueCount { get { return VariableHandlerBase.SetValueCount; } }

        public bool IsValueChanged { get { return VariableHandlerBase.IsValueChanged; } set { VariableHandlerBase.IsValueChanged = value; } }

        protected abstract void InitHandler();

        public abstract void SetAssetValue();

        private void OnEnable() { if (Application.isPlaying) { InitHandler(); } }

        public void ForceNotifyAndReset() { IsValueChanged = true; NotifyAndResetIfChanged(); }

        public void NotifyAndResetIfChanged() { VariableHandlerBase.NotifyAndResetIfChanged(); }

        public void ResetSetValueCount() { VariableHandlerBase.ResetSetValueCount(); }
    }

    public abstract class CommonVariableAsset<T, TUnityEvent> : CommonVariableAssetBase
        where TUnityEvent : UnityEvent<T>
    {
        [SerializeField]
        private T value;
        [SerializeField]
        private TUnityEvent onChange;

        private CommonVariableHandler<T> handler;

        public sealed override CommonVariableHandlerBase VariableHandlerBase { get { return Handler; } }
        public CommonVariableHandler<T> Handler { get { InitHandler(); return handler; } }

        protected sealed override void InitHandler()
        {
            if (handler == null)
            {
                handler = CommonVariable.Get<T>(HandlerName, enableDebugMessage);
                handler.OnChange += OnChange;
            }
        }

        public sealed override void SetAssetValue() { SetValue(value); }

        public void SetValue(T value) { Handler.SetValue(value); }

        public void SetValueWithoutNotify(T value) { Handler.SetValueWithoutNotify(value); }

        private void OnChange() { if (onChange != null) { onChange.Invoke(Handler.CurrentValue); } }
    }
}
