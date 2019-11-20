using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject objectToFollow;
    public float offsetLeft;
    public float offsetDown;
    public bool lockX;
    public bool lockY;
    public GameObject boundingBox;

    void LateUpdate() {

        float objectX = objectToFollow.transform.position.x;
        float x = transform.position.x;
        float objectY = objectToFollow.transform.position.y;
        float y = transform.position.y;

        // the way the movement is handled, allows the camera to move only if you're passing the center of the screen, 
        // or are past the offset, this way the camera will not lock onto you and makes it smoother overall
        if (!lockX) {
            // move the camera if the object is passed the center, or left of the offset
            if (objectX > x) {
                x = objectX;
            } else if (objectX < (x - offsetLeft)) {
                x = objectX + offsetLeft;
            }
        }
        if (!lockY) {
            // move the camera if the object is passed the center, or below the offset
            if (objectY > y) {
                y = objectY;
            } else if (objectY < (y - offsetDown)) {
                y = objectY + offsetDown;
            }
        }

        // check the bounds are not ouside of the game area

        float height = GetComponent<Camera>().orthographicSize;
        // careful of the order below, Screen.width is int not float so will be rounded
        float width = height * Screen.width / Screen.height;

        float leftBound = boundingBox.transform.position.x - (boundingBox.transform.localScale.x / 2);
        float rightBound = boundingBox.transform.position.x + (boundingBox.transform.localScale.x / 2);
        float lowerBound= boundingBox.transform.position.y - (boundingBox.transform.localScale.y / 2);
        float upperBound = boundingBox.transform.position.y + (boundingBox.transform.localScale.y / 2);

        if (x - width < leftBound) {
            x = leftBound + width;
        } else if (x + width > rightBound) {
            x = rightBound - width;
        }
        if (y - height < lowerBound) {
            y = lowerBound + height;
        } else if (y + height > upperBound) {
            y = upperBound - height;
        }

        transform.position = new Vector3(x, y, transform.position.z);

    }
}
