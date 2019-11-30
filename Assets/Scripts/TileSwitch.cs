using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.PostProcessing;
public class TileSwitch : MonoBehaviour
{
	public Tilemap[] tileStates; // for the two maps of the game world
	public int activeTileSet = 0;

	public Tile ladder;
    private bool dying = false;

    GameObject player;
	public PostProcessVolume vol;
	LensDistortion distortion;
	ColorGrading hue;
	ColorParameter col;
	private bool canSwap = true; //only allow a world shift if this is true
	private bool isShifting = false; // this keeps track of whether the player is previewing the other state

	public float swapTransitionTime = 0.25f; //how long does it take to do the swapping transition
	public float maxLensDistort = 30f;
	public float minAlpha = 0.2f; //transparency of the inactive layer
	public float hueChange = 30f;

	public float frontAlph = 1;
	private float backAlph = 0.1f;
	public GameObject crate;
    public string[] badTileNames = { "jungleTilemap_9", "jungleTilemap_19", "jungleTilemap_8", "jungleTilemap_18" }; //all the tiles that will kill us

    void Start()
    {
		backAlph = PlayerPrefs.GetFloat("alpha", 0.1f);
		minAlpha = backAlph;
        player = GameObject.Find("Player");
		activeTileSet = 0;
		col = new ColorParameter();
		UpdateTileCollider();
		SetAlpha(frontAlph, backAlph);
		vol.profile.TryGetSettings(out distortion);
		vol.profile.TryGetSettings(out hue);
		SetHue(255 - hueChange, 0);
        ResetColliders();
	}

    public void ResetColliders()
    {
        //for some reason upon launching a new scene unity is still using the tilemap collider of the old level whilst showing us the new one
        //to mitigate this I'm just adding a random function that disables and renables the collider of tiles1 to stop this weird bug
        TilemapCollider2D tiles1 = GameObject.Find("Tiles").GetComponent<TilemapCollider2D>();
		TilemapCollider2D tiles2 = GameObject.Find("Tiles2").GetComponent<TilemapCollider2D>();
        tiles1.enabled = false;
        tiles1.enabled = true;
		tiles2.enabled = true;
		tiles2.enabled = false;

    }
    public void TileAlpha(Tilemap map, float alpha) //for a specific tilemap set the alpha of the colour
    {
        Color mapCol = map.color;
        mapCol.a = alpha;
        map.color = mapCol;
    }
    // trying overloaded method for changing alpha of sprites
    public void TileAlpha(GameObject obj, float alpha) //for a specific tilemap set the alpha of the colour
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        Color mapCol = sr.color;
        mapCol.a = alpha;
        sr.color = mapCol;
    }
    public void SetAlpha(float foregroundAlpha, float backGroundAlpha)  //set the alpha value of the active tileset and the inactive one.
	{
		TileAlpha(tileStates[activeTileSet], foregroundAlpha);
        TileAlpha(tileStates[1 - activeTileSet], backGroundAlpha);
        
        foreach (Transform t in tileStates[activeTileSet].transform) {
            TileAlpha(t.gameObject, foregroundAlpha);
            if (t.gameObject.CompareTag("Projectile")) {
                t.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        foreach (Transform t in tileStates[1-activeTileSet].transform) {
            TileAlpha(t.gameObject, backGroundAlpha);
            if (t.gameObject.CompareTag("Projectile")) {
                t.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

    }

    public void SetAlphaChildren()
    {
        foreach (Transform t in tileStates[activeTileSet].transform)
        {
            TileAlpha(t.gameObject,Input.GetKey(KeyCode.LeftShift)? backAlph:frontAlph);
            if (t.gameObject.CompareTag("Projectile"))
            {
                t.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        foreach (Transform t in tileStates[1 - activeTileSet].transform)
        {
            TileAlpha(t.gameObject,Input.GetKey(KeyCode.LeftShift) ? frontAlph:backAlph);
            if (t.gameObject.CompareTag("Projectile"))
            {
                t.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public void SpawnAlpha()
    {
        SetAlpha(frontAlph, backAlph);
    }

    public void SwitchTileState()
	{
		activeTileSet = 1 - activeTileSet; 
		StartCoroutine(SwapEffect());
	}

	private void UpdateTileCollider()
	{
		tileStates[activeTileSet].gameObject.GetComponent<TilemapCollider2D>().enabled = true;
		var forwardPos = tileStates[activeTileSet].transform.position;
		tileStates[activeTileSet].gameObject.transform.position = new Vector3(forwardPos.x, forwardPos.y, 0);
		tileStates[1-activeTileSet].gameObject.transform.position = new Vector3(forwardPos.x, forwardPos.y, 1);
		tileStates[1-activeTileSet].gameObject.GetComponent<TilemapCollider2D>().enabled = false;
	}

	private bool LegalSwap()
	{
		//returns whether or not we are in a position that allows swapping or not.
		if (gameObject.GetComponent<PlayerController>().Grounded())
		{
			Vector3Int belowPoint = tileStates[activeTileSet].WorldToCell(transform.position + Vector3.down / 2);
			//string name = tileStates[activeTileSet].GetTile(belowPoint).name;
			//if(name=="jungleTilemap_1" || name == "jungleTilemap_4"){
			//	return true;
			//}
		}
		return true; //this should be return false but for now we will just allow changing whenever

	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Return) && canSwap && LegalSwap())
		{
			SwitchTileState();
			GameObject.Find("AudioController").GetComponent<AudioController>().SwitchSound();
		}
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			isShifting = true;
			SetAlpha(backAlph, frontAlph);
		}
		if (Input.GetKeyUp(KeyCode.LeftShift) && isShifting)
		{
			SetAlpha(frontAlph, backAlph);
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			if (SceneLoader.CurrentScene() == SceneLoader.firstLevelIndex)
			{
				player.transform.position = new Vector3(-23, -2, 0);
				GameObject cr = GameObject.FindGameObjectWithTag("crate");
				Destroy(cr.gameObject);
				Instantiate(crate, new Vector3(80.5f, 3.5f, 0), Quaternion.identity);
			}
			else
			{
				SceneLoader.ReloadCurrentScene();
			}
			
		}
		//touching ladder
		bool onladder = false;
		if (tileStates[activeTileSet].GetTile(tileStates[activeTileSet].WorldToCell(transform.position)) != null)
		{
			// Debug.Log(tileStates[activeTileSet].GetTile(tileStates[activeTileSet].WorldToCell(transform.position)).name);
			onladder = tileStates[activeTileSet].GetTile(tileStates[activeTileSet].WorldToCell(transform.position)).name == "jungleTilemap_28";
		}
		if(onladder)
		{
			gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 0);		
			if (Mathf.Abs(Input.GetAxis("Vertical"))>0.3f)
			{
				transform.position += Vector3.up * (Input.GetAxis("Vertical")<0? -1:1) * Time.deltaTime * 3f;
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			}
		}
	}
	private void SetHue(float value, int setting) //setting tells us blue or red, value is by how much
	{
		//goes between 255 and 220
		Color col = new Color(setting == 1 ? value : 255, setting == 0 ? value : 255, 255);
		hue.colorFilter.value = col/255f;
	}
	IEnumerator SwapEffect()
	{
		isShifting = false;
		canSwap = false;
		var alph = frontAlph;
		var hueStart = 255-hueChange;

		UpdateTileCollider();

		while ( distortion.intensity.value > -maxLensDistort)
		{
			distortion.intensity.value -=  maxLensDistort * Time.deltaTime / swapTransitionTime;
			alph -= (1 - minAlpha) * Time.deltaTime / (swapTransitionTime);
			hueStart += hueChange * Time.deltaTime / (swapTransitionTime);
			SetHue(hueStart, 1 - activeTileSet);
			SetAlpha(1 + minAlpha - alph,alph);
			yield return new WaitForEndOfFrame();
		}
		hueStart = 255;
		SetAlpha(frontAlph, backAlph);

		while (distortion.intensity.value < 0)
		{
			hueStart -= hueChange * Time.deltaTime / (swapTransitionTime);
			SetHue(hueStart,activeTileSet);
			distortion.intensity.value += maxLensDistort * Time.deltaTime / swapTransitionTime;
			yield return new WaitForEndOfFrame();
		}
		canSwap = true;
	}

    public void CheckPosition()
    {
        //if you shift whilst on top of a tile die - need to do this for water and lava too
        TileBase hitTile = tileStates[activeTileSet].GetTile(tileStates[activeTileSet].WorldToCell(player.transform.position));
        if (hitTile != null && !dying && hitTile.name != "jungleTilemap_28" && hitTile.name != "jungleTilemap_9" && hitTile.name != "jungleTilemap_19" && hitTile.name != "jungleTilemap_8" && hitTile.name != "jungleTilemap_18")
        {
            dying = true;
            player.GetComponent<PlayerController>().Die();
        }
    }

    public Tilemap GetActiveTileset() {
        return tileStates[activeTileSet];
    }
}
