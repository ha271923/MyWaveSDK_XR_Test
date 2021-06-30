using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AutoNavigation_DigitalInput : UI_AutoNavigation_TextInput
{
    public int digitalMin = 0;
    public int digitalMax = 100;
    //private bool isChecked = false;

    protected override bool CheckTextContentAcceptable(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            DummyForReselect.Select();
            CurrentInputField.Select();
            return false;
        }

        int digitalText = (int) Convert.ChangeType(text, typeof(int));          
        if(digitalText < digitalMin || digitalText > digitalMax)           
        {              
            DummyForReselect.Select();               
            CurrentInputField.Select();               
            return false;            
        }            
        else            
        {             
            return true;       
        }
    }
}
