using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;


public class AudioController : MonoBehaviour
{
	AudioClip[][] pairs;
	public AudioClip[] tracks;
    public AudioSource soundtrackA, soundtrackB;
	private static float musicvol;
	private static float soundvol;

    private int audioState = 0;

    Tilemap tileset1;
    Tilemap tileset2;

    private Vector3[] lavaTiles;
    private Vector3[] waterTiles;

    private Vector3 vecMin;
    private Vector3 vecMax;
    private Vector3Int cellMin;
    private Vector3Int cellMax;

    GameObject bounds;
    GameObject player;
    TileSwitch tileSwitch;

	// Start is called before the first frame update
	private void Awake()
	{
		GameObject[] multiAudio = GameObject.FindGameObjectsWithTag("audio");
		if (multiAudio.Length > 1)
		{
			Destroy(gameObject);
		}
		
	}

	void chooseNewSountrack(int clipIndex = -1)
	{
		int newIndex;
		if (clipIndex == -1)
		{
			newIndex = Random.Range(0, pairs.Length);
		}
		else
		{
			newIndex = clipIndex;
		}
		soundtrackA.clip = pairs[newIndex][0];
		soundtrackB.clip = pairs[newIndex][1];
		soundtrackA.Play();
		soundtrackB.Play();
	}
	void Start()
    {
		pairs = new AudioClip[tracks.Length / 2][];
		for(int i = 0; i < pairs.Length; i++)
		{
			pairs[i] = new AudioClip[2];
			pairs[i][0] = tracks[2 * i];
		    pairs[i][1] = tracks[2 * i + 1];
		}
		musicvol = PlayerPrefs.GetFloat("MusicVolume", 1);
		soundvol = PlayerPrefs.GetFloat("SoundVolume",1);
      
        soundtrackB.volume = 0;
        soundtrackA.volume = musicvol;
		soundtrackA.Play();
		soundtrackB.Play();

        if (SceneLoader.inGame())
        {
            InitDynamicVolume();
        }
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += onSceneChange;

    }

    public void SetVolume()
    {
        musicvol = PlayerPrefs.GetFloat("MusicVolume", 1);
        soundvol = PlayerPrefs.GetFloat("SoundVolume", 1);

        soundtrackB.volume = 0;
        soundtrackA.volume = musicvol;
    }

	public void SettingsVolume()
	{
		musicvol = PlayerPrefs.GetFloat("MusicVolume", 1);
		soundvol = PlayerPrefs.GetFloat("SoundVolume", 1);

		if(soundtrackA.volume > 0)
		{
			soundtrackA.volume = musicvol;
			soundtrackB.volume = 0;
		}
		else
		{
			soundtrackB.volume = musicvol;
			soundtrackA.volume = 0;
		}
	}

    void onSceneChange(Scene scene, LoadSceneMode mode)
    {
        if (SceneLoader.inGame())
        {
			if (soundtrackA.clip != tracks[(scene.buildIndex % 4) * 2]) chooseNewSountrack(scene.buildIndex % 4);//StartCoroutine(ChangeTrack(scene.buildIndex % 4));
			InitDynamicVolume();
        }
    }

    void InitDynamicVolume()
    {

        tileSwitch = GameObject.Find("Player").GetComponent<TileSwitch>();
        bounds = GameObject.Find("CameraBounds");
        player = GameObject.Find("Player");
        tileset1 = GameObject.Find("Tiles").GetComponent<Tilemap>();
        tileset2 = GameObject.Find("Tiles2").GetComponent<Tilemap>();
        tileSwitch = player.GetComponent<TileSwitch>();
        vecMin = bounds.transform.position - bounds.transform.localScale / 2;
        vecMax = vecMin + bounds.transform.localScale;
        FindTilePositions();
		StartCoroutine(SoundChecker());
	}

	IEnumerator SoundChecker()
	{
		yield return new WaitForEndOfFrame();
		if (soundtrackA.volume < musicvol)
		{
			SwitchSound();
		}
	}

    void FindTilePositions()
    {
        waterTiles = findType(tileset1, new string[] { "jungleTilemap_9", "jungleTilemap_19" }).ToArray();
        lavaTiles = findType(tileset2, new string[] { "jungleTilemap_8", "jungleTilemap_18" }).ToArray();
    }

    List<Vector3> findType(Tilemap tiles, string[] names)
    {
        var outList = new List<Vector3>();
        cellMin = tiles.WorldToCell(vecMin);
        cellMax = tiles.WorldToCell(vecMax);

        for (int i = cellMin.x; i < cellMax.x; i++)
        {
            for (int j = cellMin.y; j < cellMax.y; j++)
            {
                var cell = new Vector3Int(i, j, 0);
                foreach (var name in names)
                {
                    if (tiles.GetTile(cell) != null && tiles.GetTile(cell).name == name)
                    {
                        outList.Add(tiles.CellToWorld(cell));
                    }
                }
            }
        }
        return outList;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SceneLoader.inGame())
        {
            AdjustDynamicVolume();
        }
		if(!soundtrackA.isPlaying && !soundtrackB.isPlaying)
		{
			//StartCoroutine(ChangeTrack());
			chooseNewSountrack();
		}
    }

    void AdjustDynamicVolume()
    {
        var soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1);
        var closest = tileSwitch.activeTileSet == 0 ? DistanceToNearest(waterTiles) : DistanceToNearest(lavaTiles);
        if (tileSwitch.activeTileSet == 0)
        {
            AudioController.ChangeVolume("Water", Mathf.Clamp(1f / closest, 0, 1) * soundVolume);
            AudioController.ChangeVolume("Lava", 0f);
        }
        else
        {
            AudioController.ChangeVolume("Lava", Mathf.Clamp(1f / closest, 0, 1) * soundVolume);
            AudioController.ChangeVolume("Water", 0f);
        }
    }


    float DistanceToNearest(Vector3[] tiles)
    {
        float minDist = -1;
        foreach (var tile in tiles)
        {
            var distance = Mathf.Max(Vector3.Distance(player.transform.position, tile)-0.5f,0);
            if (minDist == -1 || distance < minDist)
            {
                minDist = distance;
            }
        }
        return minDist;
    }

    // Update is called once per frame
    void Update() {

    }

    public void SwitchSound() {
        audioState = 1 - audioState;
        StartCoroutine(AudioTransitionEnumerator());
    }

    IEnumerator AudioTransitionEnumerator() {
        float transitionTime = tileSwitch.swapTransitionTime;

        AudioSource playing = audioState == 0 ? soundtrackA : soundtrackB;
        AudioSource muted = audioState == 0 ? soundtrackB : soundtrackA;

        while (muted.volume > 0.05f) {
            playing.volume += musicvol * Time.deltaTime / transitionTime;
            muted.volume -= musicvol * Time.deltaTime / transitionTime;
            yield return new WaitForEndOfFrame();
        }
        playing.volume = musicvol;
        muted.volume = 0;
    }

	IEnumerator ChangeTrack(int newAudioClipIndex = -1)
	{
		float transitionTime = 0.5f;
		AudioSource playing = audioState == 0 ? soundtrackA : soundtrackB;
		//AudioSource playing = audioState == 0 ? soundtrackB : soundtrackA;

		while (playing.volume > 0.05f)
		{
			playing.volume -= musicvol * Time.deltaTime / transitionTime;
			yield return new WaitForEndOfFrame();
		}
		chooseNewSountrack(newAudioClipIndex);
		while(playing.volume < musicvol)
		{
			playing.volume += musicvol * Time.deltaTime / transitionTime;
		}
		playing.volume = musicvol;
	}

	public static void PlaySound(string clip) {
        AudioSource audioSource = GameObject.Find(clip).GetComponent<AudioSource>();
		audioSource.volume = soundvol;
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
