using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class EmissivePulse : MonoBehaviour
{
    public float pulseSpeed = 3f;    // how fast it pulses
    public float maxIntensity = 3f;    // peak emission intensity
    private Material mat;
    private Color baseColor;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;
        baseColor = mat.GetColor("_EmissionColor");
    }

    void Update()
    {
        // sinusoidal pulse between 0 and maxIntensity
        float emission = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f) * maxIntensity;
        Color finalColor = baseColor * emission;
        mat.SetColor("_EmissionColor", finalColor);
    }
}
