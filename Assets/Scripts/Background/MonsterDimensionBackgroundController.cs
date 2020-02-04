using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDimensionBackgroundController : MonoBehaviour
{
	private GameObject player;
	Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
		player = GameObject.Find("Player");
		startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		transform.position = startPosition - (startPosition - player.transform.position)/21 ;
	}
}
