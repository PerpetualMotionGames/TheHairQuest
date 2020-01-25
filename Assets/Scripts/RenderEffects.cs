using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
    public void SetSaturation(float amount)
    {
        hue.saturation.value = amount;
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
        //the colour will be a 255 range but we need a 0-1 range for the hue
        hue.colorFilter.value = col / 255f;
    }
}
