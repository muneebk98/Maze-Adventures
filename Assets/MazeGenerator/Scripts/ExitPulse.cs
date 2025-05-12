using UnityEngine;

public class ExitPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float scaleAmplitude = 0.1f;
    public float floatAmplitude = 0.2f;
    Vector3 baseScale;
    Vector3 basePos;

    void Start()
    {
        baseScale = transform.localScale;
        basePos = transform.position;
    }

    void Update()
    {
        float t = Mathf.Sin(Time.time * pulseSpeed);
        transform.localScale = baseScale * (1 + scaleAmplitude * t);
        transform.position = basePos + Vector3.up * (floatAmplitude * t);
    }
}
