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
    void Start()
    {
		playerControl = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
		lives = playerControl.GetHealth();
		UpdateUI();
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
}
