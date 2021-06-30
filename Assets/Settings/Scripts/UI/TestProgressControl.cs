using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestProgressControl : MonoBehaviour
{

    
    public float rate = 1.0f;
    private float curPos = 0.0f;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.material = Instantiate<Material>(GetComponent<Image>().material);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        curPos += Time.deltaTime* rate;
        image.material.SetFloat("_Pos", curPos);
        image.enabled = false;
        image.enabled = true;
    }
}
