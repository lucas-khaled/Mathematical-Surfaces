using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GPUGraph : MonoBehaviour
{
    public enum TransitionMode { CYCLE, RANDOM };

    [SerializeField]
    ComputeShader computeShader;
    [SerializeField] [Range(10, maxResolution)]
    private int resolution = 10;

    [SerializeField] 
    private Material material;
    [SerializeField] 
    private Mesh mesh;

    [SerializeField]
    private bool hasTransition = false;

    [SerializeField, Min(0f)]
    private float functionDuration = 1f;
    [SerializeField, Min(0f)]
    private float transitionDuration = 1f;

    [SerializeField] 
    private FunctionType function = 0;
    [SerializeField]
    private TransitionMode functionTransition = TransitionMode.CYCLE;

    public int Resolution { get => resolution; set => resolution = value; }
    public float FunctionDuration { get => functionDuration; set => functionDuration = value; }
    public float TransitionDuration { get => transitionDuration; set => transitionDuration = value; }
    public FunctionType FunctionType { get => function; set => function = value; }
    public TransitionMode FunctionTransition { get => functionTransition; set => functionTransition = value; }
    public bool HasTransition { get => hasTransition; set => hasTransition = value; }

    public Action<FunctionType> OnFunctionChanged;

    Vector3 scale;
    float step;
    float duration;
    bool transitioning = false;
    FunctionType transitioningFunction;

    private ComputeBuffer positionsBuffer;

    private const int maxResolution = 700;

    private static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3*4);
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    private void Update()
    {
        if (hasTransition)
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
        }
        
        UpdateFunctionOnGPU();
    }

    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        
        if (transitioning) {
            computeShader.SetFloat(
                transitionProgressId,
                Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
            );
        }

        var kernelIndex = (int) function + (int)(transitioning ? transitioningFunction : function) * FunctionLibrary.FunctionCount;
        computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);
        
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f/resolution)); 
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }

    void PickNextFunction()
    {
        function = (functionTransition == TransitionMode.CYCLE) ?
            FunctionLibrary.GetNextFunctionType(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);

        OnFunctionChanged?.Invoke(function);
    }
}
