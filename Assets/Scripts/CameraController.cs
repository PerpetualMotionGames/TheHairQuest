using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float offsetLeft;
    public float offsetDown;
    public bool lockX;
    public bool lockY;
    
    private GameObject cameraBounds;
    private GameObject player;

    private void Start() {
        cameraBounds = GameObject.Find("CameraBounds");
        player = GameObject.Find("Player");
    }

    void LateUpdate() {

        float playerX = player.transform.position.x;
        float x = transform.position.x;
        float playerY = player.transform.position.y;
        float y = transform.position.y;

        // the way the movement is handled, allows the camera to move only if you're passing the center of the screen, 
        // or are past the offset, this way the camera will not lock onto you and makes it smoother overall
        if (!lockX) {
            // move the camera if the player is passed the center, or left of the offset
            if (playerX > x) {
                x = playerX;
            } else if (playerX < (x - offsetLeft)) {
                x = playerX + offsetLeft;
            }
        }
        if (!lockY) {
            // move the camera if the player is passed the center, or below the offset
            if (playerY > y) {
                y = playerY;
            } else if (playerY < (y - offsetDown)) {
                y = playerY + offsetDown;
            }
        }

        // check the bounds are not ouside of the game area

        float height = GetComponent<Camera>().orthographicSize;
        // careful of the order below, Screen.width is int not float so will be rounded
        float width = height * Screen.width / Screen.height;

        float leftBound = cameraBounds.transform.position.x - (cameraBounds.transform.localScale.x / 2);
        float rightBound = cameraBounds.transform.position.x + (cameraBounds.transform.localScale.x / 2);
        float lowerBound= cameraBounds.transform.position.y - (cameraBounds.transform.localScale.y / 2);
        float upperBound = cameraBounds.transform.position.y + (cameraBounds.transform.localScale.y / 2);

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
