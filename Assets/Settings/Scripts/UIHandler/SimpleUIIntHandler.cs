using HTC.Triton.LensUI;
using HTC.UnityPlugin.CommonEventVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleUIIntHandler : MonoBehaviour
{
    [SerializeField] private CommonVariableAssetInt savedIntVariable;
    private LensUITMPDropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<LensUITMPDropdown>();
    }

    private void OnEnable()
    {
        dropdown.value = savedIntVariable.Handler.CurrentValue;
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    public virtual void OnDropdownValueChanged(int value)
    {
        savedIntVariable.Handler.SetValue(value);
    }
}
