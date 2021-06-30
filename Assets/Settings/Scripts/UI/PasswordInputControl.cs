using HTC.Triton.LensUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordInputControl : MonoBehaviour
{
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private LensUIButton hidePasswordButton;
    [SerializeField] private Image icon;

    [SerializeField] private Sprite hideIcon;
    [SerializeField] private Sprite showIcon;

    private void OnEnable()
    {
        SetContentTypePassword();

        hidePasswordButton.OnClick += OnHidePasswordButtonClicked;
    }

    private void OnDisable()
    {
        hidePasswordButton.OnClick -= OnHidePasswordButtonClicked;
    }

    private void OnHidePasswordButtonClicked()
    {
        if (passwordInputField.contentType == TMP_InputField.ContentType.Password)
        {
            SetContentTypeStandard();
        }
        else if (passwordInputField.contentType == TMP_InputField.ContentType.Standard)
        {
            SetContentTypePassword();
        }

        passwordInputField.ForceLabelUpdate();
    }

    private void SetContentTypeStandard()
    {
        passwordInputField.contentType = TMP_InputField.ContentType.Standard;
        icon.sprite = showIcon;
    }

    private void SetContentTypePassword()
    {
        passwordInputField.contentType = TMP_InputField.ContentType.Password;
        icon.sprite = hideIcon;
    }
}
