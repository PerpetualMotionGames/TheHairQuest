using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTile : MonoBehaviour
{
    public GameObject projectile;
    public float destroyTime;
    TileSwitch tileSwitch;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Shoot(2));
        tileSwitch = GameObject.Find("Player").GetComponent<TileSwitch>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Shoot(float delay) {
        yield return new WaitForSeconds(delay);
        var item = (GameObject)Instantiate(projectile, transform.position, transform.rotation, transform.parent.transform);
        tileSwitch.SetAlphaChildren();
        Destroy(item, destroyTime);
        StartCoroutine(Shoot(delay));
    }
}
