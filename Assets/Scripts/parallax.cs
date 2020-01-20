using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
	GameObject cam;
	Vector3 camStartPosition;
	Vector3 myStartPosition;
	public float modifier;

    // Start is called before the first frame update
    void Start()
    {
		cam = GameObject.Find("Main Camera");
		camStartPosition = cam.transform.position;
		myStartPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = myStartPosition - (camStartPosition - cam.transform.position) * modifier;
    }
}
