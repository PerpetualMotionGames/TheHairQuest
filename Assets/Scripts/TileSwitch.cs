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

	public PostProcessVolume vol;
	LensDistortion distortion;
	private bool canSwap = true;

	public float swapTransitionTime = 0.25f;
	public float maxLensDistort = 30f;
	public float minAlpha = 0.2f; //transparency of the inactive layer

    void Start()
    {
		UpdateTileCollider();
		SetAlpha(tileStates[0], 1);
		SetAlpha(tileStates[1], 0.2f);
		vol.profile.TryGetSettings(out distortion);
    }

	public void SetAlpha(Tilemap map, float alpha)
	{
		Color mapCol = map.color;
		mapCol.a = alpha;
		map.color = mapCol;
	}
	public void InstantSwapAlpha()
	{
		SetAlpha(tileStates[0], tileStates[0].color.a == 1 ? 0.2f : 1);
		SetAlpha(tileStates[1], tileStates[1].color.a == 1 ? 0.2f : 1);
	}
	public void SwitchTileState()
	{
		activeTileSet += 1;
		activeTileSet = activeTileSet % tileStates.Length; //could do activeTileSet = 1-activeTileset but this way if we did ever go to having more than 2 states this is better
		StartCoroutine(SwapEffect());
	}

	private void UpdateTileCollider()
	{
		for (int i = 0; i < tileStates.Length; i++)
		{
			bool current = activeTileSet == i;
			tileStates[i].gameObject.GetComponent<TilemapCollider2D>().enabled = current;
		}
	}

	private bool LegalSwap()
	{
		//returns whether or not we are in a position that allows swapping or not.
		if (gameObject.GetComponent<PlayerController>().Grounded())
		{
			Vector3Int belowPoint = tileStates[activeTileSet].WorldToCell(transform.position + Vector3.down / 2);
			string name = tileStates[activeTileSet].GetTile(belowPoint).name;
			Debug.Log(name);
			if(name=="jungleTilemap_1" || name == "jungleTilemap_4"){
				return true;
			}
		}
		return true; //this should be return false but for now we will just allow changing whenever

	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Return) && canSwap && LegalSwap())
		{
			SwitchTileState();
			GameObject.Find("AudioController").GetComponent<AudioSwitch>().SwitchSound();
		}
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			InstantSwapAlpha();
		}
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			InstantSwapAlpha();
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		//touching ladder
		bool onladder = false;
		if (tileStates[activeTileSet].GetTile(tileStates[activeTileSet].WorldToCell(transform.position)) != null)
		{
			Debug.Log(tileStates[activeTileSet].GetTile(tileStates[activeTileSet].WorldToCell(transform.position)).name);
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

	IEnumerator SwapEffect()
	{
		var alph = 1f;
		UpdateTileCollider();
		canSwap = false;
		while ( distortion.intensity.value > -maxLensDistort)
		{
			distortion.intensity.value -=  maxLensDistort * Time.deltaTime / swapTransitionTime;
			alph -= (1-minAlpha) * Time.deltaTime / (swapTransitionTime);
			SetAlpha(tileStates[1-activeTileSet], alph);
			SetAlpha(tileStates[activeTileSet], 1.2f - alph);
			yield return new WaitForEndOfFrame();
		}
		SetAlpha(tileStates[1 - activeTileSet], .2f);
		SetAlpha(tileStates[activeTileSet], 1);

		while (distortion.intensity.value < 0)
		{
			distortion.intensity.value += maxLensDistort * Time.deltaTime / swapTransitionTime;
			yield return new WaitForEndOfFrame();
		}
		canSwap = true;
	}
}
