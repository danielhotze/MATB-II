using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
	#nullable enable
	public GameObject? loadingScreenObj;
	public Slider? slider;
	#nullable disable

	public TextAsset textAsset;
	private string currentLevel = "level1";

	private LoadJSON loadJSON = new LoadJSON();

	AsyncOperation async;

	void Start()
    {
		WebBridge.OnLevelSelected += HandleLevelSelected;
    }

	private void HandleLevelSelected(string levelName)
    {
		currentLevel = levelName;
    }

	public void loadingScene()
	{
		Debug.Log("Now Loading level: " + currentLevel);
		textAsset = Resources.Load<TextAsset>("Levels/" + currentLevel);
		string json = textAsset.text;
		loadJSON.LoadData(loadJSON, json);

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
