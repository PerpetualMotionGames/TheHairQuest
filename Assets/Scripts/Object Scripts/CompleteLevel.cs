using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteLevel : MonoBehaviour
{
	// Start is called before the first frame update
    void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		SceneLoader controller = playerController.sceneLoader;
		if (collision.gameObject.tag == "Player")
		{
			controller.SetSceneComplete();
			controller.LoadNextLevel();
		}
	}
}
