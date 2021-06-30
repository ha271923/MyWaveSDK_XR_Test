using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintMessage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        testDebugLogs();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void checkIfDebugBuild()
    {
        // Log some debug information only if this is a debug build
        if (Debug.isDebugBuild)
        {
            Debug.Log("This is a debug build!");
        }
    }

    /*
    public void LogFilterDebugToTrue()
    {
        LogFilter.Debug= true;
    }
    */

    public void setLogEnableToTrue() {
        Debug.unityLogger.logEnabled = true;
    }

    public void developerConsoleVisibleToTrue()
    {
        Debug.developerConsoleVisible = true;
    }
    public void testDebugLogs() {
        Debug.LogError("testDebugLogs: 11111");
        Debug.Log("testDebugLogs: Debug.Log()");
        Debug.LogWarning("testDebugLogs: Debug.LogWarning()");
        Debug.LogError("testDebugLogs: Debug.LogError()");
        print("testDebugLogs: print()");
    }
}
