using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Utils : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] AnimationCurve accelerationDown;
    [SerializeField] AnimationCurve accelerationUp;
#pragma warning restore 0649

    public static Utils Instance { get; private set; }
    
    public static AnimationCurve AccelerationDown { get; private set; }
    public static AnimationCurve AccelerationUp { get; private set; }

    private void Awake()
    {
        Instance = this;

        AccelerationDown = accelerationDown;
        AccelerationUp   = accelerationUp;
    }

    public IEnumerator GraphicSetAlpha(Graphic graphic, float alpha, AnimationCurve curve, float duration, Action callback = null)
    {
        Color a = graphic.color;
        Color b = new Color(a.r, a.g, a.b, alpha);

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            graphic.color = Color.Lerp(a, b, curve.Evaluate(t / duration));
            yield return null;
        }

        graphic.color = b;
        callback?.Invoke();
    }
}
