using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        Vector2 velocity = rb2d.velocity;
        velocity.x = -10;
        rb2d.velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
