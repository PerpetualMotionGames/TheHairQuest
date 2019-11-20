using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowController : MonoBehaviour {

    public GameObject objectToFollow;
    public float offsetLeft;
    public bool lockY;

    void LateUpdate() {

        float objectX = objectToFollow.transform.position.x;
        float x = transform.position.x;

        if (objectX > x)
        {
            transform.position = new Vector3(objectX, transform.position.y, transform.position.z);
        }
        else if (objectX < (x - offsetLeft))
        {
            transform.position = new Vector3(objectX + offsetLeft, transform.position.y, transform.position.z);
        }
    }
}
