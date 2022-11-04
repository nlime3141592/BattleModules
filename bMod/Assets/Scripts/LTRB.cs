using System;
using UnityEngine;

[Serializable]
public struct LTRB
{
    public float l;
    public float t;
    public float r;
    public float b;

    public float dx => (r - l) * 0.5f; // cx - px
    public float dy => (t - b) * 0.5f; // cy - py
    public float sx => r + l;
    public float sy => t + b;

    public void Randomize(System.Random prng, float min, float max)
    {
        l = min + (min - max) * (float)prng.NextDouble();
        t = min + (min - max) * (float)prng.NextDouble();
        r = min + (min - max) * (float)prng.NextDouble();
        b = min + (min - max) * (float)prng.NextDouble();
    }

    public void DrawGizmo(Transform transform, UnityEngine.Color color)
    {
        Vector3 p = transform.position + new Vector3(dx, dy, 0.0f);
        Vector3 s = new Vector3(sx, sy, 0.0f);

        Gizmos.color = color;
        Gizmos.DrawWireCube(p, s);
    }
}