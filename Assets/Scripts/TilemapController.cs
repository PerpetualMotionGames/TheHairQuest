using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    public RenderEffects renderEffects;
    public Tilemap[] tilemaps;
    //which tilemap is currently active.
    private int activemap = 0;

    //tell the script which of the tiles fall into these categories
    public Tile[] liquid;
    public Tile[] stairs;
    public Tile[] ladder;
    public Tile[] wall;

    private GameObject player;
    private bool preview = false;
    private bool isShifting = false;

    // how long does it take to perform a shift
    public float dimensionShiftTime;
    //amount of lens distortion on shift
    public float maxLensDistort;
    // alpha of inactive layer
    public float inactiveAlpha;
    // hue variation between dimensions
    public float hueChange;
    // grain amount on preview
    public float previewGrain;
    //saturation on preview
    public float previewSaturation;
    // what alpha to give the player when in preview mode
    public float previewPlayerAlpha;



    /// ////////////////////////////////////////////////
    /// INITITALISATIONS
    /// ////////////////////////////////////////////////

    void Start()
    {
        //by default the back layer will have 0.1 alpha
        inactiveAlpha = PlayerPrefs.GetFloat("alpha", 0.1f);
        player = GameObject.Find("Player");
        ResetColliders();
    }

    private void ResetColliders()
    {
        //for some reason when launching a new scene in unity,
        //where tilemaps are prefabs,the game often uses the image of the new tilemap with the collider of the old one,
        //to resolve this issue, at the beginning of every scene, simply disable and re-enable the colliders
        TilemapCollider2D tiles1 = GameObject.Find("Tiles").GetComponent<TilemapCollider2D>();
        TilemapCollider2D tiles2 = GameObject.Find("Tiles2").GetComponent<TilemapCollider2D>();
        tiles1.enabled = false;
        tiles2.enabled = false;
        tiles1.enabled = true;
        tiles2.enabled = true;
    }


    /// ////////////////////////////////////////////////
    /// UPDATE / FIXED UPDATE
    /// ////////////////////////////////////////////////

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartPreview();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            EndPreview();
        }
		if (Input.GetKeyUp(KeyCode.Return))
		{
			ShiftDimension();
		}
    }
    /// //////////////////////////////////////////////// 
    /// PUBLIC METHODS
    /// ////////////////////////////////////////////////

    public bool OnLadder()
    {
        return CurrentTile(activemap) == "ladder";
    }

    //returns the type of the tile that the player is standing over in a given dimension, for example
    //am i stood over a ladder in the active dimension or over a wall in the inactive one?
    public string CurrentTile(int layer)
    {
        TileBase hitTile = tilemaps[layer].GetTile(PlayerPositionMap(layer));
        if (hitTile == null)
        {
            return null;
        }
        if (TileSetContains(liquid, hitTile))
        {
            return "liquid";
        }
        if(TileSetContains(wall, hitTile))
        {
            return "wall";
        }
        if (TileSetContains(stairs, hitTile))
        {
            return "stairs";
        }
        if (TileSetContains(ladder, hitTile))
        {
            return "ladder";
        }
        
        return hitTile.name;
    }

	public void ShiftDimension()
	{
		if (CanSwap())
		{
			isShifting = true;
			StartCoroutine(ShiftDimensionTransition());
		}
		else
		{
			Debug.Log("Cant swap");
		}
	}

    /// //////////////////////////////////////////////// 
    /// PRIVATE METHODS
    /// //////////////////////////////////////////////// 

    private void SetTileAlpha(Tilemap map, float alpha)
    {
        //sets the alpha of a given tilemap
        var newColor = map.color;
        newColor.a = alpha;
        map.color = newColor;
    }

    private void StartPreview()
    {
        //whichever the active dimension is, we now want to swap the alpha states
        SetTileAlpha(tilemaps[activemap], inactiveAlpha);
        SetTileAlpha(tilemaps[1 - activemap], 1);
        //add some grain for effect and saturation
        renderEffects.SetGrain(previewGrain);
        renderEffects.SetSaturation(previewSaturation);
        //make player invisible
        RenderEffects.SetAlpha(player, previewPlayerAlpha);
        //stop the player from moving
        Time.timeScale = 0;
        //set preview bool to true
        preview = true;
    }
    private void EndPreview()
    {
        //now we undo all the changes made in preview
        SetTileAlpha(tilemaps[activemap], 1);
        SetTileAlpha(tilemaps[1 - activemap], inactiveAlpha);
        renderEffects.SetGrain(0);
        renderEffects.SetSaturation(0);
        RenderEffects.SetAlpha(player, 1);
        Time.timeScale = 1;
        preview = false;
    }

    private bool TileSetContains(Tile[] tileList, TileBase check)
    {
        foreach(TileBase tile in tileList)
        {
            if (tile == check)
            {
                return true;
            }
        }
        return false;
    }

    private Vector3Int PlayerPositionMap(int layer)
    {
        return tilemaps[layer].WorldToCell(player.transform.position);
    }

    private bool CanSwap()
    {
        var conditions = new List<bool>();
        conditions.Add(isShifting == false);
        conditions.Add(preview == false);
        conditions.Add(CurrentTile(1 - activemap) != "walls" && CurrentTile(1 - activemap) != "liquid");

        //if any condition is not satisfied return false
        foreach(var condition in conditions)
        {
            if (!condition)
            {
                return false;
            }
        }
        //otherwise return true, a swap is fine
        return true;
    }

    /// ////////////////////////////////////////////////
    /// ENUMERATORS
    /// ////////////////////////////////////////////////

    IEnumerator ShiftDimensionTransition()
    {
		//set the alpha values over time.
		StartCoroutine(RenderEffects.SetAlphaOverTime(tilemaps[activemap], inactiveAlpha, dimensionShiftTime));
		StartCoroutine(RenderEffects.SetAlphaOverTime(tilemaps[1-activemap], 1, dimensionShiftTime));

		//renderEffects.SetEffectOverTime("hue", 30, dimensionShiftTime / 2, true);
		//renderEffects.SetEffectOverTime("grain", 1, dimensionShiftTime / 2, true);
		StartCoroutine(renderEffects.SetEffectOverTime("distortion", 30, dimensionShiftTime / 2, true));
		//half way through the alpha transition we update the colliders
		yield return new WaitForSeconds(dimensionShiftTime / 2); 
		//the actual swap
		activemap = 1 - activemap;
		tilemaps[activemap].gameObject.GetComponent<TilemapCollider2D>().enabled = true;
		tilemaps[1 - activemap].gameObject.GetComponent<TilemapCollider2D>().enabled = false;

		//wait for effects to complete
		yield return new WaitForSeconds(dimensionShiftTime / 2);
		isShifting = false;
    }

}
