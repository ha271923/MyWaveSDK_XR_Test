using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidFrameworkApi : MonoBehaviour
{
    static public void triggerVibration() {
        Debug.Log("triggerVibration !!!");
        Handheld.Vibrate();
    }
}
