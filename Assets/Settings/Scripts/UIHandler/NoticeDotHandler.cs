using HTC.UnityPlugin.CommonEventVariable;
using UnityEngine;

public class NoticeDotHandler : MonoBehaviour
{
    [SerializeField] private GameObject noticeDot;
    [SerializeField] private CommonVariableAssetBool commonVariableBool;

    private void OnEnable()
    {
        OnVariableChanged();
        if(commonVariableBool == null) return;
        if(commonVariableBool.Handler == null) return;
        commonVariableBool.Handler.OnChange += OnVariableChanged;
    }

    private void OnDisable()
    {
        if(commonVariableBool == null) return;
        if(commonVariableBool.Handler == null) return;
        commonVariableBool.Handler.OnChange -= OnVariableChanged;
    }

    private void OnVariableChanged()
    {
        if(noticeDot == null) return;
        if(commonVariableBool == null) return;
        if(commonVariableBool.Handler == null) return;
        if(commonVariableBool.Handler.CurrentValue == null) return;

        if(commonVariableBool.Handler.CurrentValue != null) noticeDot.SetActive(commonVariableBool.Handler.CurrentValue);
    }
}
