using HTC.Triton.LensUI;
using HTC.UnityPlugin.CommonEventVariable;
using UnityEngine;

public class UIBoolHandler : MonoBehaviour
{
    [SerializeField] protected CommonEventAssetBool eventHandler;
    [SerializeField] protected CommonVariableAssetBool currentVariableHandler;
    [SerializeField] protected CommonVariableAssetBool variableReceiver;

    protected LensUIToggleSwitch toggleSwitch;

    private void Awake()
    {
        toggleSwitch = GetComponent<LensUIToggleSwitch>();
    }

    private void OnEnable()
    {
        UpdateState();
        if(variableReceiver == null) return;
        if(variableReceiver.Handler == null) return;
        variableReceiver.Handler.OnChange += OnValueChanged;

        toggleSwitch.Initialize();
        toggleSwitch.IsOn = currentVariableHandler.Handler.CurrentValue;
        toggleSwitch.onValueChanged.AddListener(OnUIToggleValueChanged);

    }

    private void OnDisable()
    {
        if(variableReceiver == null) return;
        if(variableReceiver.Handler == null) return;
        variableReceiver.Handler.OnChange -= OnValueChanged;
        toggleSwitch.onValueChanged.RemoveListener(OnUIToggleValueChanged);
    }

    protected virtual void OnValueChanged()
    {

    }

    protected virtual void UpdateState()
    {
        if(currentVariableHandler == null) return;
        if(currentVariableHandler.Handler == null) return;
        if(variableReceiver == null) return;
        if(variableReceiver.Handler == null) return;
        if (currentVariableHandler.Handler.CurrentValue == variableReceiver.Handler.CurrentValue)
        {
            return;
        }

        currentVariableHandler.Handler.SetValue(variableReceiver.Handler.CurrentValue);
    }

    public virtual void OnUIToggleValueChanged(bool value)
    {
        currentVariableHandler.Handler.SetValue(value);
        eventHandler.Trigger1(currentVariableHandler.Handler.CurrentValue);
    }
}
