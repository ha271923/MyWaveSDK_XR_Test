using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AutoNavigation_YearInput : UI_AutoNavigation_DigitalInput
{
    [SerializeField] protected TMP_InputField dayInputField;
    [SerializeField] protected TMP_InputField monthInputField;

    private int day, month;

    public override void StartAutoNavigation()
    {
        day = int.Parse(dayInputField.text);
        month = int.Parse(monthInputField.text);

        base.StartAutoNavigation();
    }

    protected override bool CheckTextContentAcceptable(string text)
    {
        bool isAcceptable = base.CheckTextContentAcceptable(text);
        if(isAcceptable)
        {
            int year = int.Parse(text);
            int maxDay = DateTime.DaysInMonth(year, month);
            if (day > maxDay) dayInputField.text = maxDay.ToString();
        }
        return isAcceptable;
    }
}
