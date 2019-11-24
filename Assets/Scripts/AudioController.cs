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

        boxCollision = GameObject.Find("BoxCollision").GetComponent<AudioSource>();
        gameOver = GameObject.Find("GameOver").GetComponent<AudioSource>();
        jump = GameObject.Find("Jump").GetComponent<AudioSource>();
        lava = GameObject.Find("Lava").GetComponent<AudioSource>();
        playerHit = GameObject.Find("PlayerHit").GetComponent<AudioSource>();
        pushBox = GameObject.Find("PushBox").GetComponent<AudioSource>();
        splash = GameObject.Find("Splash").GetComponent<AudioSource>();
        water = GameObject.Find("Water").GetComponent<AudioSource>();

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
        switch (clip) {
            case "BoxCollision":
                boxCollision.Play();
                break;
            case "GameOver":
                gameOver.Play();
                break;
            case "Jump":
                jump.Play();
                break;
            case "Lava":
                lava.Play();
                break;
            case "PlayerHit":
                playerHit.Play();
                break;
            case "PushBox":
                pushBox.Play();
                break;
            case "Splash":
                splash.Play();
                break;
            case "Water":
                water.Play();
                break;
        }
    }

}
