using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class ForceArea : MonoBehaviour
{
    [Header("Force Area Bases")]
    public AreaMode mode;
    public float inner = 0.5f;
    public float outer = 0.0f;
    public Color gizmoColor = Color.yellow;

    [Header("Box Area Options")]
    public LTRB ltrb;
    [Header("Circle Area Options")]
    public Vector2 circleOffset;
    public float radius;

    public enum AreaMode
    {
        Box, Circle
    }

    private void OnEnable()
    {
        ForceAreaManager.Subscribe(this);
    }

    private void OnDisable()
    {
        ForceAreaManager.Unsubscribe(this);
    }

    private void OnDrawGizmos()
    {
        Color color_backup = Gizmos.color;
        Gizmos.color = gizmoColor;

        switch(mode)
        {
            case AreaMode.Box:
                float px = transform.position.x + 0.5f * (ltrb.r - ltrb.l);
                float py = transform.position.y + 0.5f * (ltrb.t - ltrb.b);
                float sx = ltrb.l + ltrb.r;
                float sy = ltrb.b + ltrb.t;
                Gizmos.DrawWireCube(new Vector3(px, py, transform.position.z), new Vector3(sx, sy, 0.0f));
                break;
            case AreaMode.Circle:
                Gizmos.DrawWireSphere(transform.position + new Vector3(circleOffset.x, circleOffset.y, 0.0f), radius);
                break;
            default:
                break;
        }

        Gizmos.color = color_backup;
    }
}