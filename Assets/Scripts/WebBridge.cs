using UnityEngine;
using System.Runtime.InteropServices;

public class WebBridge : MonoBehaviour
{
    public string currentLevel;

    [DllImport("__Internal")]
    public static extern void RequestDifficultyLevel();

    public delegate void LevelSelectedEventHandler(string levelName);
    public static event LevelSelectedEventHandler OnLevelSelected;

    public void SetLevel(string levelName)
    {
        currentLevel = levelName;
        Debug.Log("Level set to: " + levelName);
        OnLevelSelected?.Invoke(levelName);
    }

    [DllImport("__Internal")]
    public static extern void SendPerformanceData(string data);

    [DllImport("__Internal")]
    public static extern void QuitGame();
}
