﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Report : MonoBehaviour
{
    public List<Text> texts;
    public Serializer serializer;
    public GameObject workloadScore;
    public static List<int> scores = new List<int> { 0, 0, 0, 0, 0};

    // Start is called before the first frame update
    void Start()
    {
        calculateScore();
        displayScore();
        #if UNITY_WEBGL
            WebBridge.SendPerformanceData(scores.ToString());
        #endif
        //calculate score of each task
        serializer.saveReport(scores);
    }

    public void displayScore()
    {
        for(int i=0; i<4; i++)
        {
            texts[i].text = scores[i].ToString();
        }
    }

    public void calculateScore()
    {
        //Debug.Log(SystemMonitoring.score[0]);
        scores[0] = (SystemMonitoring.score[1] != 0) ? (((SystemMonitoring.score[0])*100) / SystemMonitoring.score[1]) : 0;
        scores[1] = (Tracking.score[1] != 0) ? (((Tracking.score[0])*100) / Tracking.score[1]) : 0;
        scores[2] = (CommunicationsTask.score[1] != 0) ? (((CommunicationsTask.score[0])*100) / CommunicationsTask.score[1]) : 0;
        scores[3] = calculateResourceManagementScore();
    }

    private int calculateResourceManagementScore() //As per MATB, checks if both tanks are in +- 500 of starting value, and grades accordingly,
    {
        float tank1deviation = Mathf.Sqrt(Mathf.Pow(ResourceManagement.initialCapacity[0] - ResourceManagement.tankCapacity[0][1], 2));//Applying Deviation formula
        //Debug.Log("1" + tank1deviation);
        float tank2deviation = Mathf.Sqrt(Mathf.Pow(ResourceManagement.initialCapacity[1] - ResourceManagement.tankCapacity[1][1], 2));//Applying Deviation formula
        //Debug.Log("1" + tank2deviation);
        float score = 100;
        if(tank1deviation < 500f)
        {
            tank1deviation = tank1deviation / 10f;
            //Debug.Log("2" + tank1deviation);
            score -= tank1deviation;
        }
        else
        {
            score -= 50;
        }
        if (tank2deviation < 500f)
        {
            tank2deviation = tank2deviation / 10f;
            //Debug.Log("2" + tank2deviation);
            score -= tank2deviation;
        }
        else
        {
            score -= 50;
        }

        int s = (int) score;

        return s;
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();

    #endif
    }
}
