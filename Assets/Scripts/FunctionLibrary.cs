using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public enum FunctionType { WAVE, MULTI_WAVE, RIPPLE, SPHERE, TORUS }

public static class FunctionLibrary
{
    public delegate Vector3 Function(float x, float z, float t, float amplitude = 1f, float frequency = 1f, float morphingRate = 1);

    static Function[] functions = { Wave, MultiWave, Ripple, Sphere, Torus };

    public static int FunctionCount => functions.Length;

    public static Function GetFunction(FunctionType type) =>functions[(int)type];

    public static FunctionType GetNextFunctionType(FunctionType type) => ((int)type < functions.Length-1) ? type + 1 : 0;
    

    public static FunctionType GetRandomFunctionNameOtherThan(FunctionType type)
    {
        FunctionType choice = (FunctionType)Random.Range(1, functions.Length - 1);
        return choice == type ? 0 : choice;
    }

    public static Vector3 Morph(float u, float v, float t, Function from, Function to, float progress)
    {
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
    }

    public static Vector3 Wave(float u, float v, float t, float amplitude = 1f, float frequency = 1f, float morphingRate = 1)
    {
        Vector3 p;
        p.x = u;
        p.y = amplitude * Sin((u + v + t) * PI * frequency);
        p.z = v;
        return p;
    }

    public static Vector3 MultiWave(float u, float v, float t, float amplitude = 1f, float frequency = 1f, float morphingRate = 1)
    {
        Vector3 p;
        p.x = u;
        p.y = amplitude * Sin((u + t*morphingRate) * PI * frequency);
        p.y += amplitude * Sin((v + t) * PI * frequency*2) * 0.5f;
        p.y += amplitude * Sin((u + v + t * morphingRate*0.5f) * PI * frequency);
        p.y = p.y * (2f / 3f);
        p.z = v;
        return p;
    }

    public static Vector3 Ripple(float u, float v, float t, float amplitude = 1, float frequency = 1, float morphingRate = 1)
    {
        Vector3 p;
        float d = Sqrt(u*u + v*v);
        p.x = u;
        p.y = amplitude*Sin((4f *d-t) * PI * frequency);
        p.y = p.y / (1f + 10f * d);
        p.z = v;
        return p;
    }

    public static Vector3 Sphere(float u, float v, float t, float amplitute = 1, float frequency = 1, float morphing = 1)
    {
        Vector3 p;
        float r = amplitute * (0.9f + 0.1f * Sin(PI * (12.0f * u + 8.0f * v + t)*Clamp(frequency, 1, 3)));
        float s = r * Cos(0.5f * PI * v);
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s *  Cos(PI * u);
        return p;
    }

    public static Vector3 Torus(float u, float v, float t, float amplitute = 1, float frequency = 1, float morphing = 1)
    {
        float r1 = 0.7f + 0.1f * Sin(PI * (8.0f * u + 0.5f * t)*Clamp(frequency, 1, 2)) * amplitute;
        float r2 = 0.15f + 0.05f * Sin(PI * (16.0f * u + 8.0f * v + 3.0f * t)* Clamp(frequency, 1, 2)) * amplitute;
        float s = r1+r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
}
