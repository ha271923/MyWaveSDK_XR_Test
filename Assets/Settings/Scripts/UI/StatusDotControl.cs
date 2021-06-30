using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusDotControl : MonoBehaviour
{
    public Sprite[] DotTextures;

    private Image dotImage;

    private void Awake()
    {
        dotImage = GetComponent<Image>();
    }

    public void UpdateDot(bool isConnected)
    {
        if (isConnected)
        {
            dotImage.sprite = DotTextures[1];
        }
        else
        {
            dotImage.sprite = DotTextures[0];
        }
    }

    public void OnPaired()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void OnNotPaired()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
