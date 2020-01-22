using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillObject : MonoBehaviour
{
    Color spriteColor;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayerKill()
    {
        StartCoroutine(FadeOut());
    }

    public void Kill()
    {
        StartCoroutine(Die());
    }

    IEnumerator FadeOut()
    {
        AudioController.PlaySound("PlayerHit");
        spriteColor = gameObject.GetComponent<SpriteRenderer>().color;
        float deathTime = 0.5f;
        Color newCol = spriteColor;
        while (Mathf.Max(newCol.r, newCol.g, newCol.b) > 0)
        {
            newCol = new Color(newCol.r - Time.deltaTime / deathTime, newCol.g - Time.deltaTime / deathTime, newCol.b - Time.deltaTime / deathTime);
            gameObject.GetComponent<SpriteRenderer>().color = newCol;
            yield return new WaitForEndOfFrame();
        }
        SceneLoader.ReloadCurrentScene();
    }
    IEnumerator Die()
    {
        spriteColor = gameObject.GetComponent<SpriteRenderer>().color;
        float deathTime = 0.5f;
        Color newCol = spriteColor;
        while (Mathf.Max(newCol.r, newCol.g, newCol.b) > 0)
        {
            newCol = new Color(newCol.r - Time.deltaTime / deathTime, newCol.g - Time.deltaTime / deathTime, newCol.b - Time.deltaTime / deathTime);
            gameObject.GetComponent<SpriteRenderer>().color = newCol;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
