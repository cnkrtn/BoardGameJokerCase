using UnityEngine;
using System;
using System.Collections;

public class LerpHelper : MonoBehaviour
{
    // Easing Functions
    public static float EaseLinear(float t) { return t; }
    public static float EaseInQuad(float t) { return t * t; }
    public static float EaseOutQuad(float t) { return t * (2f - t); }
    public static float EaseInOutQuad(float t) { return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t; }
    public static float EaseInCubic(float t) { return t * t * t; }
    public static float EaseOutCubic(float t) { return (--t) * t * t + 1f; }
    public static float EaseInOutCubic(float t) { return t < 0.5f ? 4f * t * t * t : (t - 1f) * (2f * t - 2f) * (2f * t - 2f) + 1f; }

    // Custom easing function for yoyo effect
    public static float EaseYoyo(float t) 
    {
        return t <= 0.5f ? t * 2f : 2f * (1f - t);
    }

    // Position Lerp for RectTransform
    public static IEnumerator LerpPosition(RectTransform rectTransform, Vector3 startPosition, Vector3 endPosition, float duration, Func<float, float> easingFunction)
    {
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            t = easingFunction(t);
            rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = endPosition;
    }

    // Overloaded Position Lerp with RectTransform
    public static IEnumerator LerpPosition(RectTransform rectTransform, Vector3 startPosition, RectTransform endRectTransform, float duration, Func<float, float> easingFunction)
    {
        Vector3 endPosition = endRectTransform.localPosition;
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            t = easingFunction(t);
            rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = endPosition;
    }

    // Position Lerp for normal Transform with Vector3 positions
    public static IEnumerator LerpPosition(Transform transform, Vector3 startPosition, Vector3 endPosition, float duration, Func<float, float> easingFunction)
    {
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            t = easingFunction(t);
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition;
    }

    // Position Lerp for normal Transform with Transform positions
    public static IEnumerator LerpPosition(Transform startTransform, Transform endTransform, float duration, Func<float, float> easingFunction)
    {
        Vector3 startPosition = startTransform.position;
        Vector3 endPosition = endTransform.position;
        return LerpPosition(startTransform, startPosition, endPosition, duration, easingFunction);
    }

    // Custom Scale Lerp with Yoyo effect
    public static IEnumerator LerpScaleYoyo(RectTransform rectTransform, Vector3 startScale, Vector3 peakScale, float duration)
    {
        float halfDuration = duration / 2;
        
        // Scaling up
        float time = 0;
        while (time < halfDuration)
        {
            float t = time / halfDuration;
            t = EaseInOutQuad(t);
            rectTransform.localScale = Vector3.Lerp(startScale, peakScale, t);
            time += Time.deltaTime;
            yield return null;
        }
        
        // Scaling down
        time = 0;
        while (time < halfDuration)
        {
            float t = time / halfDuration;
            t = EaseInOutQuad(t);
            rectTransform.localScale = Vector3.Lerp(peakScale, startScale, t);
            time += Time.deltaTime;
            yield return null;
        }
        
        rectTransform.localScale = startScale;
    }

    // Color Fade Lerp
    public static IEnumerator LerpColor(Material material, Color startColor, Color endColor, float duration, Func<float, float> easingFunction)
    {
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            t = easingFunction(t);
            material.color = Color.Lerp(startColor, endColor, t);
            time += Time.deltaTime;
            yield return null;
        }
        material.color = endColor;
    }

    // Helper to get Easing Function by Type
    public static Func<float, float> GetEasingFunction(EasingFunctionType type)
    {
        switch (type)
        {
            case EasingFunctionType.Linear: return EaseLinear;
            case EasingFunctionType.EaseInQuad: return EaseInQuad;
            case EasingFunctionType.EaseOutQuad: return EaseOutQuad;
            case EasingFunctionType.EaseInOutQuad: return EaseInOutQuad;
            case EasingFunctionType.EaseInCubic: return EaseInCubic;
            case EasingFunctionType.EaseOutCubic: return EaseOutCubic;
            case EasingFunctionType.EaseInOutCubic: return EaseInOutCubic;
            case EasingFunctionType.EaseYoyo: return EaseYoyo;
            default: return EaseLinear;
        }
    }
}

public enum EasingFunctionType
{
    Linear,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseYoyo,
    // Add more as needed
}
