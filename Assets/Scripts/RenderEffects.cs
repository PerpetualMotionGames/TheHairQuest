using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Tilemaps;
public class RenderEffects : MonoBehaviour
{
    public PostProcessVolume vol;
    LensDistortion distortion;
    ColorGrading hue;
    ColorParameter colParam;
    Grain grain;

    private void Start()
    {
        //initialise the post processing parameters
        colParam = new ColorParameter();
        vol.profile.TryGetSettings(out distortion);
        vol.profile.TryGetSettings(out hue);
        vol.profile.TryGetSettings(out grain);
    }

    public static void SetAlpha(GameObject obj, float alpha)
    {
        SpriteRenderer spr = obj.GetComponent<SpriteRenderer>();
        var newColor = spr.color;
        newColor.a = alpha;
        spr.color = newColor;
    }
	//overload method tiles
	public static void SetAlpha(Tilemap tile, float alpha)
	{
		var newColor = tile.color;
		newColor.a = alpha;
		tile.color = newColor;
	}
	public static void SetChildrenAlpha(GameObject obj, float alpha)
    {
        foreach(Transform t in obj.transform)
        {
            SpriteRenderer spr = t.gameObject.GetComponent<SpriteRenderer>();
            var newColor = spr.color;
            newColor.a = alpha;
            spr.color = newColor;
        }
    }
    public static IEnumerator SetAlphaOverTime(GameObject obj, float alpha, float time)
    {
        SpriteRenderer spr = obj.GetComponent<SpriteRenderer>();
        //amount to increment alpha each frame
        var step = (spr.color.a - alpha)/time;
        float nowTime = Time.time;
        while(Time.time-nowTime < time)
        {
            SetAlpha(obj, spr.color.a + step*Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
	//overload method used for tilesets
	public static IEnumerator SetAlphaOverTime(Tilemap tile, float alpha, float time)
	{
		//amount to increment alpha each frame
		var startAlph = tile.color.a;
		var step = (alpha-startAlph) / time;
		float nowTime = Time.time;
		while (Time.time - nowTime < time)
		{
			SetAlpha(tile, startAlph + step * (Time.time-nowTime));
			yield return new WaitForEndOfFrame();
		}
	}
	public void SetSaturation(float amount)
    {
        hue.saturation.value = amount;
    }
	//works for distortion, grain and hue, and sets the amount desired over the given period of time
	public IEnumerator SetEffectOverTime(string effectName, float amount, float time, bool reverseEffectAfter=false)
	{
		var effect = hue.saturation;

		if (effectName == "distortion")
		{
			effect =distortion.intensity;
		}
		else if(effectName == "grain")
		{
			effect = grain.intensity;
		}
		else if (effectName != "hue")
		{
			Debug.Log("Error: Unspecifed effect was asked for");
			yield break;
		}
		//reverseEffectAfter does pretty much what it says, it will make said transition over time, and then over the same period of time do the reverse.
		var startSaturation = effect.value;
		var nowTime = Time.time;
		var increment = (amount - startSaturation) / time;
		while (Time.time - nowTime < time)
		{
			effect.value += increment * Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		effect.value = amount;
		if (reverseEffectAfter)
		{
			//if we want to reverse the effect after
			nowTime = Time.time;
			while (Time.time - nowTime < time)
			{
				effect.value -= increment * Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			effect.value = startSaturation;
		}
	}

    public void SetDistortion(float amount)
    {
        distortion.intensity.value = amount;
    }

    public void SetGrain(float amount)
    {
        grain.intensity.value = amount;
    }

    public void SetHue(Color col)
    {
        //the colour will be a 255 range but we need a 0-1 range for the hue so just divide
        hue.colorFilter.value = col / 255f;
    }
}
