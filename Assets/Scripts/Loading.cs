using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Loading : MonoBehaviour
{
	#nullable enable
	public GameObject? loadingScreenObj;
	public Slider? slider;
	#nullable disable

	public TextAsset textAsset;

	private LoadJSON loadJSON = new LoadJSON();

	AsyncOperation async;

	public void loadingScene()
	{
		textAsset = Resources.Load("Config") as TextAsset;
		string json = textAsset.text;
		loadJSON.LoadData(loadJSON, json);
		//string json = JsonUtility.ToJson(loadJSON, true);
		//loadJSON.WriteToFile(json); //used to generate the sample file, comment the file read part in LoadJSON and uncomment the placeholder tasks and these lines(2).
		StartCoroutine(AsynchronousLoad("TasksScene"));
	}

	public void toReport()
	{
		StartCoroutine(AsynchronousLoad("Report"));
	}

	IEnumerator AsynchronousLoad(string scene)
	{
		if(loadingScreenObj)
        {
			loadingScreenObj.SetActive(true);
		}
		async = SceneManager.LoadSceneAsync(scene);
		async.allowSceneActivation = false; // this stops changing to next scene 

		while(async.isDone == false)
		{
			if(slider) slider.value = async.progress;
			if (async.progress == 0.9f)
			{
				if(slider) slider.value = 1f;
				Cursor.lockState = CursorLockMode.None;
				async.allowSceneActivation = true;// changed to true when scene is completely loaded 
			}
			yield return null;
		}


	}
}
