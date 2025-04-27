using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Graph : MonoBehaviour
{
    public enum TransitionMode { CYCLE, RANDOM };

    [Header("Awake Values")]
    [SerializeField]
    private Transform pointPrefab = default;
    [SerializeField]
    private int numberOfPoints = 10;

    [Header("Transition Values")]
    [SerializeField, Min(0f)]
    private float functionDuration = 1f;
    [SerializeField, Min(0f)]
    private float transitionDuration = 1f;


    [Header("Witch Function")]
    [SerializeField] 
    private FunctionType function = 0;
    [SerializeField]
    private TransitionMode functionTransition = TransitionMode.CYCLE;

    [Header("Common Wave Values")]
    [SerializeField] [Range(0.1f,2)]
    private float amplitude = 1;
    [SerializeField] [Range(0.1f,5)]
    private float frequency = 1;
    [SerializeField] [Range(0.1f,7)]
    private float size = 1;

    [Header("MultiWave Values")]
    [SerializeField] [Range(0.1f, 1)]
    private float morphingRate = 1;

    Transform[] points;
    Vector3 scale;
    float step;
    float duration;
    bool transitioning = false;
    FunctionType transitioningFunction;

    private void Awake()
    {
        points = new Transform[numberOfPoints * numberOfPoints];
        for(int i = 0; i<points.Length; i++)
        {
            Transform point = Instantiate(pointPrefab);
            point.SetParent(transform, false);
            points[i] = point;
        }
    }

    private void Update()
    {
        duration += Time.deltaTime;

        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitioningFunction = function;
            PickNextFunction();
        }

        if (transitioning)
            UpdateFunctionTransition();
        else
            UpdateFunction();
    }

    void PickNextFunction()
    {
        function = (functionTransition == TransitionMode.CYCLE) ?
            FunctionLibrary.GetNextFunctionType(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }

    private void UpdateFunction()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        float time = Time.time;
        step = (2f / numberOfPoints);
        scale = Vector3.one * (step * size);
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == numberOfPoints)
            {
                x = 0;
                z += 1;
            }

            Transform point = points[i];
            point.localScale = scale;

            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;

            point.localPosition = f(u, v, time, amplitude, frequency, morphingRate);
        }
    }

    private void UpdateFunctionTransition()
    {
        FunctionLibrary.Function to = FunctionLibrary.GetFunction(function);
        FunctionLibrary.Function from = FunctionLibrary.GetFunction(transitioningFunction);

        float progress = duration / transitionDuration;
        float time = Time.time;
        step = (2f / numberOfPoints);
        scale = Vector3.one * step * size;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == numberOfPoints)
            {
                x = 0;
                z += 1;
            }

            Transform point = points[i];
            point.localScale = scale;

            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;

            point.localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
        }
    }
}
