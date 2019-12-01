using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Projectile : MonoBehaviour
{
    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = this.GetComponentInParent<Tilemap>();
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        Vector2 velocity = rb2d.velocity;
        velocity.x = -10;
        rb2d.velocity = velocity;
    }

    void FixedUpdate()
    {
        //tilemap = this.GetComponentInParent<Tilemap>();
        TileBase hitTile = tilemap.GetTile(tilemap.WorldToCell(transform.position));
        if (hitTile != null && tilemap.GetColliderType(tilemap.WorldToCell(transform.position)) != Tile.ColliderType.None) {
            Destroy(gameObject);
        }
    }
}
