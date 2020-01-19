using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paralax : MonoBehaviour
{
	Vector3 startPos;
	GameObject target;
	Vector3 mystart;
	public float modifier;
    // Start is called before the first frame update
    void Start()
    {
		target = GameObject.Find("Main Camera");
		startPos = target.transform.position;
		mystart = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		transform.position = mystart - (startPos - target.transform.position)*modifier;
    }
}
