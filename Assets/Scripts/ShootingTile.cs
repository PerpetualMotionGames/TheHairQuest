using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTile : MonoBehaviour
{
    public GameObject projectile;
    public float destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Shoot(2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Shoot(float delay) {
        yield return new WaitForSeconds(delay);
        var item = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
        Destroy(item, destroyTime);
        StartCoroutine(Shoot(delay));
    }
}
