using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwitch : MonoBehaviour
{
	public AudioSource soundtrackA;
	public AudioSource soundtrackB;
	TileSwitch tileswap;

	private int audioState = 0;
    // Start is called before the first frame update
    void Start()
    {
		tileswap = GameObject.Find("Player").GetComponent<TileSwitch>();
		soundtrackB.volume = 0;
		soundtrackA.volume = 1;
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
			playing.volume += Time.deltaTime / transitionTime;
			muted.volume -= Time.deltaTime / transitionTime;
			yield return new WaitForEndOfFrame();
		}
		playing.volume = 1;
		muted.volume = 0;
	}
}
