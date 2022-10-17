using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class ForceArea : MonoBehaviour
{
    [Header("Area Definition")]
    public float x0;
    public float x1;
    public float x2;
    public float x3;
    public float y0;
    public float y1;
    public float wn;
    public float wx;

    private int[] idxs = new int[]{
        0,1,2,3, 0,1,3,2, 0,2,1,3, 0,2,3,1, 0,3,1,2, 0,3,2,1,
        1,0,2,3, 1,0,3,2, 1,2,0,3, 1,2,3,0, 1,3,0,2, 1,3,2,0,
        2,0,1,3, 2,0,3,1, 2,1,0,3, 2,1,3,0, 2,3,0,1, 2,3,1,0,
        3,0,1,2, 3,0,2,1, 3,1,0,2, 3,1,2,0, 3,2,0,1, 3,2,1,0
    };

    [Header("Area Debug Option")]
    public float xn;
    public float xx;
    public float yn;
    public float yx;
    public float dx;
    public float dy;

    [Header("Image Information")]
    public int width;
    public int height;
    private byte[] pixels;

    [Header("Basic Debug Option")]
    public bool checker = false;

    private void Update()
    {
        if(checker)
        {
            checker = false;

            float[] xArr = new float[]{x0, x1, x2, x3};

            for(int i = 0; i < 1; i++)
            {
                int ptr = idxs[4 * i];

                SetPixels(
                    xArr[ptr], xArr[ptr + 1], xArr[ptr + 2], xArr[ptr + 3],
                    y0, y1,
                    wx, wn
                );
            }
        }
    }

    private float GetWeight(
        float _x0, float _x1, float _x2, float _x3,
        float _y0, float _y1,
        float _wx, float _wn,
        float px, float py
        )
    {
        if(py < _y0 || py > _y1 || px < _x0 || px > _x3)
            return _wn;
        else if(px < _x1)
            return (_wx - _wn) * (px - _x0) / (_x1 - _x0);
        else if(px > _x2)
            return (_wx - _wn) * (_x3 - px) / (_x3 - _x2);
        else
            return _wx;
    }

    private void SetPixels(
        float _x0, float _x1, float _x2, float _x3,
        float _y0, float _y1,
        float _wx, float _wn
    )
    {
        width = (int)((xx - xn) / dx);
        height = (int)((yx - yn) / dy);

        pixels = new byte[width * height];

        for(int i = 0; i < height; i++)
        for(int j = 0; j < width; j++)
        {
            float px = j * dx + xn;
            float py = i * dy + yn;
            int idx = i * width + j;
            float value = GetWeight(
                _x0, _x1, _x2, _x3,
                _y0, _y1,
                _wx, _wn,
                px, py
                );
            pixels[idx] = (byte)value;
        }
    }
}