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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyUp(KeyCode.Alpha8))
		{
			lives -= 1;
		}
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
