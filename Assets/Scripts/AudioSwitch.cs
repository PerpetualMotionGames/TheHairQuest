using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwitch : MonoBehaviour
{
	public AudioSource soundtrackA;
	public AudioSource soundtrackB;
	TileSwitch tileswap;
	public float vol;

	private int audioState = 0;
    // Start is called before the first frame update
    void Start()
    {
		tileswap = GameObject.Find("Player").GetComponent<TileSwitch>();
		soundtrackB.volume = 0;
		soundtrackA.volume = vol;
    }

    // Update is called once per frame
    void Update()
    {

    }

	public void SwitchSound()
	{
		audioState = 1 - audioState;
		StartCoroutine(AudioTransitionEnumerator());
	}

	IEnumerator AudioTransitionEnumerator()
	{
		float transitionTime = tileswap.swapTransitionTime;

		AudioSource playing = audioState == 0 ? soundtrackA : soundtrackB;
		AudioSource muted = audioState == 0 ? soundtrackB : soundtrackA;

		while(muted.volume > 0.05f)
		{
			playing.volume += vol*Time.deltaTime / transitionTime;
			muted.volume -= vol*Time.deltaTime / transitionTime;
			yield return new WaitForEndOfFrame();
		}
		playing.volume = vol;
		muted.volume = 0;
	}
}
