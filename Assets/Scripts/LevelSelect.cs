using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelSelect : MonoBehaviour
{
	public Button[] levels;
	private int numLevels;
	private bool[] doneLevel;
    // Start is called before the first frame update
    void Start()
    {
		numLevels = levels.Length;
		doneLevel = new bool[numLevels];
		PlayerPrefs.SetInt("level1", 1);
		GetLevels();
		UpdateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void GetLevels()
	{
		//this will return the levels we have/haven't completed
		for(int i = 0; i < numLevels; i++)
		{
			string levelname = "level" + (i + 1);
			doneLevel[i] = PlayerPrefs.GetInt(levelname, 0) == 1; //leveli is 0 if we have not completed the previous one and 1 if we have
		}
	}

	private void LoadLevel(int index)
	{
		if (index < SceneManager.sceneCountInBuildSettings)
		{
			SceneManager.LoadScene(index);
		}
		else
		{
			SceneManager.LoadScene(0);
		}
	}

	private void UpdateButtons()
	{
		int tutorialIndex = 2;
		Debug.Log(tutorialIndex);
		for(int i = 1; i < numLevels; i++)
		{
			int buttonIndex = i;
			if (doneLevel[buttonIndex-1])
			{
				levels[buttonIndex].enabled = true;
				levels[buttonIndex].onClick.AddListener(() => LoadLevel(tutorialIndex+buttonIndex));
			}
			else
			{
				levels[buttonIndex].enabled = false;
				var buttonColors = levels[buttonIndex].colors;
				buttonColors.normalColor = new Color32(0,0,0,60);
				levels[buttonIndex].colors = buttonColors;
			}
		}
	}
}
