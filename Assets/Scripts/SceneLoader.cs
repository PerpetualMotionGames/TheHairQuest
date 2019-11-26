using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	int thisIndex; //index of current scene

	private void Start()
	{
		thisIndex = SceneManager.GetActiveScene().buildIndex;
	}
	public void LoadLevel(int index)
	{
		SceneManager.LoadScene(index);
	}

	public void ReloadCurrentScene()
	{
		SceneManager.LoadScene(thisIndex);
	}

	public void LoadNextLevel()
	{
		Debug.Log(thisIndex);
		if(thisIndex < SceneManager.sceneCountInBuildSettings - 1)
		{
			LoadLevel(thisIndex + 1);
		}
		else
		{
			Debug.Log(SceneManager.sceneCountInBuildSettings);
			LoadLevel(0);
		}
	}

	public void SetSceneComplete()
	{
		var level = thisIndex - 1; //because menu and level select are in play
		PlayerPrefs.SetInt("level" + level, 1);
	}
}
