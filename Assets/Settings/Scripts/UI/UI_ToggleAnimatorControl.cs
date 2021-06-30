using HTC.Triton.LensUI;
using UnityEngine;

public class UI_ToggleAnimatorControl : MonoBehaviour
{
    private LensUIToggleSwitch toggleSwitch;
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        toggleSwitch = GetComponent<LensUIToggleSwitch>();
        if (animator == null) animator = GetComponent<Animator>();

        animator.SetBool("IsOn", toggleSwitch.IsOn);
        toggleSwitch.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnDisable()
    {
        toggleSwitch.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool value)
    {
        animator.SetBool("IsOn", value);
    }
}
