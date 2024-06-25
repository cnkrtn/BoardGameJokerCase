using UnityEngine;
using System;
using System.Collections;

public class LerpHelper : MonoBehaviour
{
    // Easing Functions
    public static float EaseInQuad(float t) { return t * t; }
    public static float EaseOutQuad(float t) { return t * (2f - t); }
    public static float EaseInOutQuad(float t) { return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t; }
    public static float EaseInCubic(float t) { return t * t * t; }
    public static float EaseOutCubic(float t) { return (--t) * t * t + 1f; }
    public static float EaseInOutCubic(float t) { return t < 0.5f ? 4f * t * t * t : (t - 1f) * (2f * t - 2f) * (2f * t - 2f) + 1f; }

    // Position Lerp
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

    // Scale Lerp
    public static IEnumerator LerpScale(RectTransform rectTransform, Vector3 startScale, Vector3 endScale, float duration, Func<float, float> easingFunction)
    {
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            t = easingFunction(t);
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            time += Time.deltaTime;
            yield return null;
        }
        rectTransform.localScale = endScale;
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
            case EasingFunctionType.EaseInQuad: return EaseInQuad;
            case EasingFunctionType.EaseOutQuad: return EaseOutQuad;
            case EasingFunctionType.EaseInOutQuad: return EaseInOutQuad;
            case EasingFunctionType.EaseInCubic: return EaseInCubic;
            case EasingFunctionType.EaseOutCubic: return EaseOutCubic;
            case EasingFunctionType.EaseInOutCubic: return EaseInOutCubic;
            default: return EaseInQuad;
        }
    }
}

public enum EasingFunctionType
{
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    // Add more as needed
}
