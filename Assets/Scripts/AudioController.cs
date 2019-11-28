using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;


public class AudioController : MonoBehaviour
{
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
    void Start()
    {
		musicvol = PlayerPrefs.GetFloat("MusicVolume", 1);
		soundvol = PlayerPrefs.GetFloat("SoundVolume",1);
      
        soundtrackB.volume = 0;
        soundtrackA.volume = musicvol;
        soundtrackA.Play();
        soundtrackB.Play();

        if (SceneLoader.inGame())
        {
            tileSwitch = GameObject.Find("Player").GetComponent<TileSwitch>();
            InitDynamicVolume();
        }

    }

    void InitDynamicVolume()
    {
        bounds = GameObject.Find("CameraBounds");
        player = GameObject.Find("Player");
        tileset1 = GameObject.Find("Tiles1").GetComponent<Tilemap>();
        tileset2 = GameObject.Find("Tiles2").GetComponent<Tilemap>();
        tileSwitch = player.GetComponent<TileSwitch>();
        vecMin = bounds.transform.position - bounds.transform.localScale / 2;
        vecMax = vecMin + bounds.transform.localScale;
        FindTilePositions();
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
            var distance = Vector3.Distance(player.transform.position, tile);
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
