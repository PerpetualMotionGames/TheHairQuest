using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//README
//Can merge all the functions in here into the playercontroller - this is just to ensure I avoid conflicts

public class PlayerSupplement : MonoBehaviour
{
	SceneLoader sceneLoader;
	float lowerbound;
	float respawnHeight = 10f;
	GameObject player;
    // Start is called before the first frame update
    void Start()
    {
		GameObject SceneController = new GameObject("SceneController");
		sceneLoader = SceneController.AddComponent<SceneLoader>();
		var bounds = GameObject.Find("CameraBounds");
		lowerbound = bounds.transform.position.y - bounds.transform.localScale.y/2;
		player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		CheckBounds();
    }

	void CheckBounds()
	{
		if (player.transform.position.y < lowerbound - respawnHeight)
		{
			Respawn();
		}
	}
	
	void Respawn()
	{
		sceneLoader.ReloadCurrentScene();
	}
}
