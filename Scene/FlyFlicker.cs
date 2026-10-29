using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class StarTwinkle : MonoBehaviour
{
    [Header("Grundhelligkeit")]
    public float baseAlpha = 0.6f;
    public Color baseColor = Color.white;

    [Header("Twinkle (Aufleuchten)")]
    public float glowAlpha = 1f;          
    public float glowIntensity = 1.5f;   
    public float twinkleDuration = 0.4f;  

    [Header("Timing zwischen Funkeln")]
    public float minDelay = 2f;          
    public float maxDelay = 6f;           

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        SetColor(baseColor, baseAlpha);
        StartCoroutine(TwinkleLoop());
    }

    IEnumerator TwinkleLoop()
    {
        yield return new WaitForSeconds(Random.Range(0f, maxDelay));

        while (true)
        {
            yield return StartCoroutine(DoTwinkle());
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator DoTwinkle()
    {
        float half = twinkleDuration / 2f;
        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float lerp = t / half;
            float alpha = Mathf.Lerp(baseAlpha, glowAlpha, lerp);
            Color c = baseColor * Mathf.Lerp(1f, glowIntensity, lerp);
            SetColor(c, alpha);
            yield return null;
        }
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float lerp = t / half;
            float alpha = Mathf.Lerp(glowAlpha, baseAlpha, lerp);
            Color c = baseColor * Mathf.Lerp(glowIntensity, 1f, lerp);
            SetColor(c, alpha);
            yield return null;
        }

        SetColor(baseColor, baseAlpha);
    }

    void SetColor(Color rgb, float alpha)
    {
        Color c = rgb;
        c.a = alpha;
        sr.color = c;
    }
}