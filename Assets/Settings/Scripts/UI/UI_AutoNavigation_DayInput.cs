using System;
using TMPro;

public class UI_AutoNavigation_DayInput : UI_AutoNavigation_DigitalInput
{
    public override void StartAutoNavigation()
    {
        var monthInputField = (TMP_InputField)PreviousUI;
        int month = int.Parse(monthInputField.text);

        digitalMin = 1;
        digitalMax = DateTime.DaysInMonth(2020, month);     // Any leap year to set 29 days in Feb.

        base.StartAutoNavigation();
    }
}
