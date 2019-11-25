using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
public class TileSwitch : MonoBehaviour
{
	public Tilemap[] tileStates; // for the two maps of the game world
	private int activeTileSet = 0;

	public Tile ladder;

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
	public float backAlph = 0.2f;

    void Start()
    {
		col = new ColorParameter();
		UpdateTileCollider();
		SetAlpha(frontAlph, backAlph);
		vol.profile.TryGetSettings(out distortion);
		vol.profile.TryGetSettings(out hue);
		SetHue(255 - hueChange, 0);
	}

	public void TileAlpha(Tilemap map, float alpha) //for a specific tilemap set the alpha of the colour
	{
		Color mapCol = map.color;
		mapCol.a = alpha;
		map.color = mapCol;
	}
	public void SetAlpha(float foregroundAlpha, float backGroundAlpha)  //set the alpha value of the active tileset and the inactive one.
	{
		TileAlpha(tileStates[activeTileSet], foregroundAlpha);
		TileAlpha(tileStates[1-activeTileSet], backGroundAlpha);
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
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
			if (Input.GetKey(KeyCode.UpArrow))
			{
				transform.position += Vector3.up * Time.deltaTime * 3f;
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
}
