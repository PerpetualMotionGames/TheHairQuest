using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
	public Slider soundSlider;
	public Slider musicSlider;
    // Start is called before the first frame update
    void Start()
    {
		soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1);
		musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void UpdateParameters()
	{
		UpdateMusic();
		UpdateSound();
		
	}

	public void UpdateMusic()
	{
		PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
	}
	public void UpdateSound()
	{
		PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
	}
}
