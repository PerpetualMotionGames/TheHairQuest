using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    //the background images move according to camera position.
	GameObject cam;
    //need to get initial positions to base movement off.
	Vector3 camStartPosition;
	Vector3[] myStartPosition;

    //each background image gets a modifier which movement is based off
    public GameObject[] backGroundImages;
    public float[] modifiers;

    void Start()
    {
		cam = GameObject.Find("Main Camera");
		camStartPosition = cam.transform.position;
        myStartPosition = new Vector3[backGroundImages.Length];

        for (int i = 0; i < backGroundImages.Length; i++)
        {
            myStartPosition[i] = backGroundImages[i].transform.position;
        }

        //every image needs a modifier so if the modifier list is not long enough, make it
        if(modifiers.Length < backGroundImages.Length)
        {
            var newModifiers = new float[backGroundImages.Length];
            for(int i = 0; i < backGroundImages.Length; i++)
            {
                //any modifiers that are not set will default to zero
                newModifiers[i] = i < modifiers.Length ? modifiers[i] : 0;
            }
        }
    }

    void Update()
    {
        //shift each of the background images to be inline with the camera based on their shift modifier.
        for(int i = 0; i < backGroundImages.Length; i++)
        {
            backGroundImages[i].transform.position = myStartPosition[i] - (camStartPosition - cam.transform.position) * modifiers[i];
        }
        
    }
}
