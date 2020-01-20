using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScrolling : MonoBehaviour
{
    //the images that make up the scrolling background - MUST BE AT LEAST 3
    //as they move to the right or left the final element of the set moves to be the first one for continual movement
    public GameObject[] bgSet;
    //the speed at which the tiles moves
    public float moveSpeed;
    //calculate the width of the tesselating tiles
    float width;
    float leftPoint;
    float rightPoint;
    //if the leftmost image reaches the orginal position of its righthand neighbour, the rightmost image becomes the leftmost one
    //similarly if the final image reaches the original image of the penultimate image, the leftmost image becomes the rightmost one
    int penultimateIndex;
    

    void Start()
    {
        penultimateIndex = bgSet.Length - 2;
        //calculate positions used to check background position against 
        leftPoint = bgSet[1].transform.position.x;
        rightPoint = bgSet[penultimateIndex].transform.position.x;

        width = bgSet[1].transform.position.x - bgSet[0].transform.position.x;
    }

    void Update()
    {
        //scroll the images and then check if a shift needs to take place
        Scroll();
        Check();
    }

    void Scroll()
    {
        foreach(var obj in bgSet)
        {
            obj.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }
    }

    void Check()
    {
        if(bgSet[0].transform.position.x >= leftPoint)
        {
            ShiftToLeft();
        }
        if(bgSet[penultimateIndex].transform.position.x <= rightPoint)
        {
            ShiftToRight();
        }
    }

    void ShiftToLeft()
    {
        //rightmost image becomes leftmost
        var temp = bgSet[bgSet.Length - 1];
        temp.transform.position -= Vector3.right * width * bgSet.Length;
        for(int i = bgSet.Length-1; i > 0; i--)
        {
            bgSet[i] = bgSet[i - 1];
        }
        bgSet[0] = temp;
    }

    void ShiftToRight()
    {
        //leftmost image becomes rightmost
        var temp = bgSet[0];
        temp.transform.position += Vector3.right * width * bgSet.Length;
        for (int i = 0; i < bgSet.Length-1; i++)
        {
            bgSet[i] = bgSet[i + 1];
        }
        bgSet[bgSet.Length-1] = temp;
    }
}
