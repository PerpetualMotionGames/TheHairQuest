using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.PostProcessing;
public class TileSwitch : MonoBehaviour
{
	public Tilemap[] tileStates; // for the two maps of the game world

	private int activeTileSet = 0;

	public PostProcessVolume vol;
	LensDistortion distortion;
	private bool canSwap = true;

	public float swapTransitionTime = 0.5f;
	public float maxLensDistort = 30f;
	public float minAlpha = 0.2f; //transparency of the inactive layer

    void Start()
    {
		UpdateTileCollider();
		vol.profile.TryGetSettings(out distortion);
    }

	public void SetAlpha(Tilemap map, float alpha)
	{
		Color mapCol = map.color;
		mapCol.a = alpha;
		map.color = mapCol;
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
		if (Input.GetKeyDown(KeyCode.Return) && canSwap &&LegalSwap())
		{
			SwitchTileState();
			LegalSwap();
		}
    }

	IEnumerator SwapEffect()
	{
		var alph = 1f;

		canSwap = false;
		while ( distortion.intensity.value > -maxLensDistort)
		{
			distortion.intensity.value -=  maxLensDistort * Time.deltaTime / swapTransitionTime;
			alph -= (1-minAlpha) * Time.deltaTime / (swapTransitionTime);
			SetAlpha(tileStates[1-activeTileSet], alph);
			SetAlpha(tileStates[activeTileSet], 1.2f - alph);
			yield return new WaitForEndOfFrame();
		}
		UpdateTileCollider();
		while (distortion.intensity.value < 0)
		{
			distortion.intensity.value += maxLensDistort * Time.deltaTime / swapTransitionTime;
			yield return new WaitForEndOfFrame();
		}
		canSwap = true;
	}
}
