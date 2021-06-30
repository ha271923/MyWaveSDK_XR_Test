using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMaterialProperty : MonoBehaviour
{
    [SerializeField] private float materialFloat = 1;
    [SerializeField] private string materialFloatName = "_Top";

    private Image img = null;
    private float materialFloatValue;

    private void Awake()
    {
        img = GetComponent<Image>();
        img.material = Instantiate<Material>(img.material);
        materialFloatValue = img.material.GetFloat(materialFloatName);
    }

    private void LateUpdate()
    {
        if (materialFloat != materialFloatValue)
        {
            materialFloatValue = materialFloat;
            img.material.SetFloat(materialFloatName, materialFloatValue);
            img.enabled = false;
            img.enabled = true;
        }
    }
}
