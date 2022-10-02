using System;

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
}