using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioController : MonoBehaviour
{
    public AudioSource soundtrackA, soundtrackB;
    private static AudioSource boxCollision, gameOver, jump, lava, playerHit, pushBox, splash, water;
    TileSwitch tileswap;
    public float vol;

    private int audioState = 0;
    // Start is called before the first frame update
    void Start() {
        tileswap = GameObject.Find("Player").GetComponent<TileSwitch>();

        soundtrackB.volume = 0;
        soundtrackA.volume = vol;
    }

    // Update is called once per frame
    void Update() {

    }

    public void SwitchSound() {
        audioState = 1 - audioState;
        StartCoroutine(AudioTransitionEnumerator());
    }

    IEnumerator AudioTransitionEnumerator() {
        float transitionTime = tileswap.swapTransitionTime;

        AudioSource playing = audioState == 0 ? soundtrackA : soundtrackB;
        AudioSource muted = audioState == 0 ? soundtrackB : soundtrackA;

        while (muted.volume > 0.05f) {
            playing.volume += vol * Time.deltaTime / transitionTime;
            muted.volume -= vol * Time.deltaTime / transitionTime;
            yield return new WaitForEndOfFrame();
        }
        playing.volume = vol;
        muted.volume = 0;
    }

    public static void PlaySound(string clip) {
        AudioSource audioSource = GameObject.Find(clip).GetComponent<AudioSource>();
        if (audioSource == null) {
            Debug.Log("AudioController.PlaySound could not find a clip named: " + clip);
            return;
        }
        audioSource.Play();
    }

    public static void ChangeVolume(string clip, float volume) {
        AudioSource audioSource = GameObject.Find(clip).GetComponent<AudioSource>();
        if (audioSource == null) {
            Debug.Log("AudioController.ChangeVolume could not find a clip named: " + clip);
            return;
        }
        audioSource.volume = volume;
        if (volume > 0 && !audioSource.isPlaying) {
            audioSource.Play();
        } else if (volume == 0 && audioSource.isPlaying) {
            audioSource.Stop();
        }
    }

}
