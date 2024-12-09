using UnityEngine;
using System.Runtime.InteropServices;

public class WebBridge : MonoBehaviour
{
    public string currentLevel;

    public delegate void LevelSelectedEventHandler(string levelName);
    public static event LevelSelectedEventHandler OnLevelSelected;

    [DllImport("__Internal")]
    private static extern void ReceivePerformanceData(string data);

    public void SetLevel(string levelName)
    {
        currentLevel = levelName;
        Debug.Log("Level set to: " + levelName);
        OnLevelSelected?.Invoke(levelName);
    }

    public void SendPerformanceData(string data)
    {
        ReceivePerformanceData(data);
    }
}
