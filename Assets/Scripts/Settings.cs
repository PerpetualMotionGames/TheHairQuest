using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
	public Slider soundSlider;
	public Slider musicSlider;
	public Slider alphaSlider;

	public Text musicText;
	public Text soundText;
	public Text alphaText;
    // Start is called before the first frame update
    void Start()
    {
		soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1);
		musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
		alphaSlider.value = PlayerPrefs.GetFloat("alpha", 0.1f);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void UpdateParameters()
	{
		UpdateMusic();
		UpdateSound();
		UpdateAlpha();
	}

	public void UpdateMusic()
	{
		musicText.text = Mathf.Round(musicSlider.value * 100) + "%";
		PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        try
        {
            GameObject.Find("AudioController").GetComponent<AudioController>().SetVolume();
        }
        catch
        {
            //nothing
        }
	}
	public void UpdateSound()
	{
		soundText.text = Mathf.Round(soundSlider.value * 100) + "%";
		PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
	}

	public void UpdateAlpha()
	{
		alphaText.text = "" + Mathf.Round(alphaSlider.value * 10) / 10f;
		PlayerPrefs.SetFloat("alpha", alphaSlider.value);
	}
}
