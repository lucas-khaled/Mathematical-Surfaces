using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Graph : MonoBehaviour
{
    public abstract int Resolution { get; set; }
    public abstract FunctionType FunctionType { get; set; }
    public abstract TransitionMode FunctionTransition { get; set; }
    public abstract bool HasTransition { get; set; }
    public abstract float TransitionDuration { get; set; }
    public abstract float FunctionDuration { get; set; }

    public abstract float ResolutionMin { get; }
    public abstract float ResolutionMax { get; }

    public Action<FunctionType> OnFunctionChanged;
}

public enum TransitionMode { CYCLE, RANDOM };
