using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicVolume : MonoBehaviour
{
	public Tilemap tileset1;
	public Tilemap tileset2;

	private Vector3[] lavaTiles;
	private Vector3[] waterTiles;

	private Vector3 vecMin;
	private Vector3 vecMax;
	private Vector3Int cellMin;
	private Vector3Int cellMax;

	GameObject bounds;
	GameObject player;
	TileSwitch tileSwitch;

	public AudioSource water;
	public AudioSource lava;

    // Start is called before the first frame update
    void Start()
    {
		bounds = GameObject.Find("CameraBounds");
		player = GameObject.Find("Player");
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
				foreach(var name in names)
				{
					if (tiles.GetTile(cell)!=null && tiles.GetTile(cell).name == name)
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
		var soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1);
		var closest = tileSwitch.activeTileSet == 0 ? DistanceToNearest(waterTiles) : DistanceToNearest(lavaTiles);
		if (tileSwitch.activeTileSet == 0)
		{
			water.volume = Mathf.Clamp(1f/closest,0,1)*soundVolume;
			lava.volume = 0f;
		}
		else
		{
			lava.volume = Mathf.Clamp(1f / closest, 0, 1)*soundVolume;
			water.volume = 0f;
		}
    }

	
	float DistanceToNearest(Vector3[] tiles)
	{
		float minDist = -1;
		foreach(var tile in tiles)
		{
			var distance = Vector3.Distance(player.transform.position, tile);
			if(minDist==-1 ||  distance<minDist)
			{
				minDist = distance;
			}
		}
		return minDist;
	}
}
