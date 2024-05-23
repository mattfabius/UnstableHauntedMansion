using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInteractableGraphics : MonoBehaviour
{
    public float fadeSpeed = 3f;
    public float fadeMin = 0.2f;
    public float fadeMax = 0.325f;
    public float offsetSpeed = 1f;

    private Material material;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;

        if (fadeMin > fadeMax)
        {
            fadeMin = fadeMax;
        }
    }

    void Update()
    {
        float fade = (((Mathf.Sin(Time.time*fadeSpeed)+1f)/2f)*(fadeMax-fadeMin))+fadeMin;
        material.SetFloat("_Fade", fade);

        material.SetFloat("_PosY", Time.time * offsetSpeed / 1000f);
    }
}
