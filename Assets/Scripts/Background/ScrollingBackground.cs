using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public GameObject singleBackgroundImage;
    public int numberOfTiles; //MUST BE AN ODD NUMBER
    public GameObject[] bgSet;

    //the speed at which the tiles moves
    public float moveSpeed;

    //calculate the width of the tesselating tiles and the threshold points for performing a shift.
    private float leftPoint;
    private float rightPoint;
    private float width;

    //if the leftmost image reaches the orginal position of its righthand neighbour, the rightmost image becomes the leftmost one
    //similarly if the final image reaches the original image of the penultimate image, the leftmost image becomes the rightmost one
    private int penultimateIndex;

    //boolean that determines if the script is active.
    private bool active = true;


    void Start()
    { 
        if (singleBackgroundImage == null && bgSet.Length==0)
        {
            //if we haven't set up any images then just return to avoid an error.
            active = false;
            return;
        }

        //width of a single image - this requires correct pixel setting in the editor.
        width = singleBackgroundImage.gameObject.transform.localScale.x;

        if (numberOfTiles % 2 == 0)
        {
            //if it is not an odd number, just make it one
            numberOfTiles += 1;
        }

        if(bgSet.Length < 3)
        {
            //if we haven't made multiple images for the scrolling background ourselves then duplicate the given one either side and use this
            InitBackGroundImageSet();
        }

        SetCheckingVariables();
        
    }

    //only needed if the background images werent set up in the editor manually.
    void InitBackGroundImageSet()
    {
        bgSet = new GameObject[numberOfTiles];
        
        var bgPosX = singleBackgroundImage.gameObject.transform.position.x;

        float[] xPositions = new float[numberOfTiles];

        for (int i = 0; i < numberOfTiles; i++)
        {
            var relativeX = (i - (numberOfTiles - 1) / 2);
            xPositions[i] = bgPosX + width * relativeX;
            if (i != (numberOfTiles - 1) / 2)
            {
                bgSet[i] = Instantiate(singleBackgroundImage, new Vector3(xPositions[i], singleBackgroundImage.transform.position.y, 0), Quaternion.identity);
            }
            else
            {
                bgSet[i] = singleBackgroundImage;
            }
        }
    }

    void SetCheckingVariables()
    {
        penultimateIndex = bgSet.Length - 2;
        leftPoint = bgSet[1].transform.position.x;
        rightPoint = bgSet[penultimateIndex].transform.position.x;
    }
    void Update()
    {
        //scroll the images and then check if a shift needs to take place and if so, do it.
        if(active)
        {
            Scroll();
            Check();
        }
        
    }

    //called every frame to make the background image scroll.
    void Scroll()
    {
        foreach (var obj in bgSet)
        {
            obj.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }
    }

    //checks if a shift needs to be performed and if so, it calls the relevant shift function
    void Check()
    {
        if (bgSet[0].transform.position.x >= leftPoint)
        {
            ShiftToLeft();
        }
        if (bgSet[penultimateIndex].transform.position.x <= rightPoint)
        {
            ShiftToRight();
        }
    }

    void ShiftToLeft()
    {
        Debug.Log("Left Shift");

        //rightmost image becomes leftmost
        var temp = bgSet[bgSet.Length - 1];
        temp.transform.position -= Vector3.right * width * bgSet.Length;
        for (int i = bgSet.Length - 1; i > 0; i--)
        {
            bgSet[i] = bgSet[i - 1];
        }
        bgSet[0] = temp;
    }

    void ShiftToRight()
    {
        Debug.Log("Right Shift");

        //leftmost image becomes rightmost
        var temp = bgSet[0];
        temp.transform.position += Vector3.right * width * bgSet.Length;
        for (int i = 0; i < bgSet.Length - 1; i++)
        {
            bgSet[i] = bgSet[i + 1];
        }
        bgSet[bgSet.Length - 1] = temp;
    }
}
