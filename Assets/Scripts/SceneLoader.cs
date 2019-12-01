using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class SceneLoader : MonoBehaviour
{
	int thisIndex; //index of current scene
	public bool levelUI = false;
	private ColorGrading colorgrad;
	private PostProcessVolume vol;
	private GameObject inGameUI;

    public static int firstLevelIndex = 4;

    public static bool inGame()
    {
        return (CurrentScene() >= firstLevelIndex && CurrentScene() < firstLevelIndex+6);
    }
	public static void LoadFirstLevel()
	{
		SceneManager.LoadScene(firstLevelIndex);
	}
	private void Start()
	{ 
		Time.timeScale = 1;
		thisIndex = SceneManager.GetActiveScene().buildIndex;
		if (levelUI)
		{
			vol = GameObject.Find("Main Camera").GetComponent<PostProcessVolume>();
			vol.profile.TryGetSettings(out colorgrad);
			inGameUI = GameObject.Find("InGameUI");
			inGameUI.SetActive(false);
		}
	}
	public void QuitGame()
	{
		Application.Quit();
	}
	public void LoadLevel(int index)
	{
		SceneManager.LoadScene(index);
	}

	public static void ReloadCurrentScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadNextLevel()
	{
		// Debug.Log(thisIndex);
		if(thisIndex < SceneManager.sceneCountInBuildSettings - 1)
		{
			LoadLevel(thisIndex + 1);
		}
		else
		{
			// Debug.Log(SceneManager.sceneCountInBuildSettings);
			LoadLevel(0);
		}
	}

	public void SetSceneComplete()
	{
		var level = thisIndex - firstLevelIndex + 1; //because menu and level select are in play
		PlayerPrefs.SetInt("level" + level, 1);
	}

	public void Pause()
	{
		
		GameObject.Find("Player").GetComponent<PlayerController>().paused = true;
		inGameUI.SetActive(true);
		colorgrad.colorFilter.value = new Color(0.08f, 0.08f, 0.08f, 1);
		Time.timeScale = 0;
	}
	public void Resume()
	{
		Time.timeScale = 1;
		GameObject.Find("Player").GetComponent<PlayerController>().paused = false;
		inGameUI.SetActive(false);
		colorgrad.colorFilter.value = new Color(1,1,1, 1);
	}

    public static int CurrentScene()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}
