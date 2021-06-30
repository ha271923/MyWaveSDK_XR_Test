using HTC.Triton.LensUI;
using HTC.UnityPlugin.CommonEventVariable;
using UnityEngine;

public class SimpleUIBoolHandler : MonoBehaviour
{
    [SerializeField] private CommonVariableAssetBool savedBoolVariable;
    private LensUIToggleSwitch toggleSwitch;

    private void Awake()
    {
        toggleSwitch = GetComponent<LensUIToggleSwitch>();
    }

    private void OnEnable()
    {
        toggleSwitch.IsOn = savedBoolVariable.Handler.CurrentValue;
        toggleSwitch.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnDisable()
    {
        toggleSwitch.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    public virtual void OnToggleValueChanged(bool value)
    {
        savedBoolVariable.Handler.SetValue(value);
    }
}
