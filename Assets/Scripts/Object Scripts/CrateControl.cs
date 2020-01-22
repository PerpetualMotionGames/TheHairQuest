using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateControl : MonoBehaviour
{
	Rigidbody2D myRb;

	public bool touchingPlayer = false;
	float threshold = 3f;
	float impactThreshold = 0.5f;
	float timegap = 1f;
	float horizontalThreshold = 1f;

	float lastPlayed = 0f;
	bool impactReady = false;
	bool playingPush = false;
    // Start is called before the first frame update
    void Start()
    {
		myRb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

		if (myRb.velocity.y < -threshold)
		{
			impactReady = true;
		}
		if (myRb.velocity.y > -impactThreshold && impactReady)
		{
			if (Time.time - lastPlayed > timegap)
			{
				AudioController.PlaySound("BoxCollision");
				lastPlayed = Time.time;
				impactReady = false;
			}

		}
	}

}
