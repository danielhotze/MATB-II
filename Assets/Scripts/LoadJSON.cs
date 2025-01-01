using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class LoadJSON 
{
    private static string configFile = "con";

    //System monitoring
    public List<IntListWrapper> System_Monitoring_Tasks = new List<IntListWrapper>();

    //Tracking
    public List<IntListWrapper> Tracking_Tasks = new List<IntListWrapper>();

    //Communications 
    public List<IntListWrapper> Communication_Tasks = new List<IntListWrapper>();

    //Resource Management
    public List<IntListWrapper> Resource_Management_Tasks = new List<IntListWrapper>();
    public List<IntListWrapper> Resource_Management_Tank_Capacity = new List<IntListWrapper>();
    public List<int> Resource_Management_Flow_Rate = new List<int>();
    public List<int> Resource_Management_Tank_Consumption = new List<int>();

    public void LoadData(LoadJSON loadJSON, string json)
    {
        loadJSON = new LoadJSON();
        JsonUtility.FromJsonOverwrite(json, loadJSON);

        //Done now forward the data
        loadJSON.passData();
    }

    public void passData()
    {
        //System Monitoring 
        foreach (IntListWrapper sysMon in System_Monitoring_Tasks)
        {
            sysMon.list[0]-= 1; // convert scale/button number (1-6) to index (0-5)
            SystemMonitoring.tasks.Add(sysMon.list);
        }

        //Tracking
        foreach(IntListWrapper TR in Tracking_Tasks)
        {
            Tracking.tasks.Add(TR.list);
        }

        //Communications 
        foreach(IntListWrapper COM in Communication_Tasks)
        {
            CommunicationsTask.tasks.Add(COM.list);
        }

        //Resource Management Pump Tasks
        foreach(IntListWrapper ResMan in Resource_Management_Tasks)
        {
            UnityEngine.Debug.Log(ResMan.list[0]);
            ResMan.list[0]-= 1; //convert pump number (1-8) to pump index (0-7)
            UnityEngine.Debug.Log(ResMan.list[0]);
            ResourceManagement.tasks.Add(ResMan.list);
        }

        foreach (IntListWrapper ResMan in Resource_Management_Tank_Capacity)
        {
            ResourceManagement.tankCapacity.Add(ResMan.list);
        }

        foreach(int ResMan in Resource_Management_Flow_Rate)
        {
            ResourceManagement.flowRates.Add(ResMan);
        }
        ResourceManagement.flowRates.Add(Resource_Management_Tank_Consumption[0]);
        ResourceManagement.flowRates.Add(Resource_Management_Tank_Consumption[1]);

    }

    public void WriteToFile(string json)
    {
        string path = GetFilePath(configFile);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        Debug.Log(path);
        using(StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }

    private string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }

    private string ReadFromFile()
    {
        string path = GetFilePath(configFile);
        Debug.Log(path);
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                var fileContents = reader.ReadToEnd();
                return fileContents;
            }
        }
        else
        {
            Debug.Log("404 not found");
            return "";
        }
    }
}

[Serializable]
public class ListWrapper<T>: IEnumerable<T>
{
    public List<T> list;

    public ListWrapper()
    {
        list = new List<T>();
    }
    public ListWrapper(IEnumerable<T> collection)
    {
        list = new List<T>(collection);
    }
    public ListWrapper(int capacity)
    {
        list = new List<T>(capacity);
    }

    public void Add(T item)
    {
        list.Add(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}


[Serializable]
public class IntListWrapper : ListWrapper<int> { }