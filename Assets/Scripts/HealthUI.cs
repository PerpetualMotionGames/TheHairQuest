using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    // Start is called before the first frame update
	int lives=3;
	public Sprite[] livesStates;
	public GameObject healthbar;
	PlayerController playerControl;
	public GameObject compass;
    void Start()
    {
		playerControl = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
		lives = playerControl.GetHealth();
		UpdateUI();
		CompassUI();
    }

	public void UpdateUI()
	{
		if (lives > 0)
		{
			healthbar.GetComponent<Image>().sprite = livesStates[lives - 1];
		}
		else
		{
			Color trans = new Color(0, 0, 0, 0);
			healthbar.GetComponent<Image>().color = trans;
		}
		
	}

	public void CompassUI()
	{
		Vector3 rotation = (GameObject.Find("LevelExit").transform.position- GameObject.Find("Player").transform.position);
		Debug.Log(rotation);
		Debug.Log(Quaternion.LookRotation(rotation));
		float angle;
		if (rotation.x > 0)
		{
			angle = Mathf.Atan(rotation.y / rotation.x) * Mathf.Rad2Deg;
		}
		else
		{
			angle = Mathf.Atan(rotation.y / rotation.x) * Mathf.Rad2Deg+180;
		}
		compass.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,angle);
	}
}
