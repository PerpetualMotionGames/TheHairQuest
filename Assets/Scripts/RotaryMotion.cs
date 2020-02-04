using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaryMotion : MonoBehaviour
{
	//rotates the given objects at the given constant rate
	public GameObject[] rotaryObjects;
	public float[] rotationConstants;

    void Start()
    {
		//first make sure we have enough constants, if not, set the missing ones to zero
		if (rotationConstants.Length < rotaryObjects.Length)
		{
			var tempArray = new float[rotaryObjects.Length];
			for(int i = 0; i < rotaryObjects.Length; i++)
			{
				tempArray[i] = i < rotationConstants.Length ? rotationConstants[i] : 0;
			}
			rotationConstants = tempArray;
		}
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < rotaryObjects.Length; i++)
		{
			rotaryObjects[i].transform.Rotate(new Vector3(0, 0, rotationConstants[i])*Time.deltaTime);
		}
    }

    //call this upon a shift of dimension to make all rotating objects rotate the other way.
	public void InvertRotation()
	{
		for(int i = 0; i < rotationConstants.Length; i++)
		{
			rotationConstants[i] *= -1;
		}
	}
}
