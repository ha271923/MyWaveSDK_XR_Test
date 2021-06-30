using UnityEngine;
using UnityEngine.UI;

public class ControllerImageControl : MonoBehaviour
{
    public Sprite[] ControllerTextures;

    private Image controllerImage;

    private void Awake()
    {
        controllerImage = GetComponent<Image>();
    }

    public void UpdateTexture(bool isConnected)
    {
        if (isConnected)
        {
            var tempColor = controllerImage.color;
            tempColor.a = 1f;
            controllerImage.color = tempColor;
        }
        else
        {
            var tempColor = controllerImage.color;
            tempColor.a = 0.5f;
            controllerImage.color = tempColor;
        }
    }

    public void OnPaired()
    {
        if (controllerImage == null)
            controllerImage = GetComponent<Image>();
        controllerImage.sprite = ControllerTextures[1];
    }

    public void OnNotPaired()
    {
        if(controllerImage == null)
            controllerImage = GetComponent<Image>();

        controllerImage.sprite = ControllerTextures[0];
    }
}
