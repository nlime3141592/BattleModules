using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ForceAreaManager
{
    private static List<ForceArea> s_m_areas;

    static ForceAreaManager()
    {
        s_m_areas = new List<ForceArea>(20);
    }

    public static void Subscribe(ForceArea area)
    {
        if(s_m_areas.Contains(area))
            return;

        s_m_areas.Add(area);
    }

    public static void Unsubscribe(ForceArea area)
    {
        if(!s_m_areas.Contains(area))
            return;

        s_m_areas.Remove(area);
    }

    public static Vector2 GetValue(Vector2 detectorPosition)
    {
        int i;
        Vector2 force = Vector2.zero;

        for(i = 0; i < s_m_areas.Count; i++)
            force += s_m_SwitchValue(detectorPosition, s_m_areas[i]);

        return force;
    }

    private static Vector2 s_m_SwitchValue(Vector2 detectorPosition, ForceArea detecteeArea)
    {
        if(detecteeArea == null)
            throw new NullReferenceException("영역 존재하지 않음.");

        switch(detecteeArea.mode)
        {
            case ForceArea.AreaMode.Box:
                return s_m_GetBoxValue(detectorPosition, detecteeArea);
            case ForceArea.AreaMode.Circle:
                return s_m_GetCircleValue(detectorPosition, detecteeArea);
            default:
                return s_m_GetBoxValue(detectorPosition, detecteeArea);
        }
    }

    private static Vector2 s_m_GetBoxValue(Vector2 detectorPosition, ForceArea detecteeArea)
    {
        float dx = detectorPosition.x - detecteeArea.transform.position.x;
        float dy = detectorPosition.y - detecteeArea.transform.position.y;
        LTRB ltrb = detecteeArea.ltrb;

        float outer = detecteeArea.outer;
        float inner = detecteeArea.inner;
        float fx = 0.0f;

        if(dx < -ltrb.l || dx > ltrb.r || dy < -ltrb.b || dy > ltrb.t)
            fx = outer;
        else if(dx < 0.0f)
            fx = (outer - inner) * (1 + dx / ltrb.l) + outer;
        else
            fx = (inner - outer) * (1 - dx / ltrb.r) + outer;

        return Vector2.right * fx;
    }

    private static Vector2 s_m_GetCircleValue(Vector2 detectorPosition, ForceArea detecteeArea)
    {
        float dx = detectorPosition.x - detecteeArea.transform.position.x;
        float dy = detectorPosition.y - detecteeArea.transform.position.y;
        LTRB ltrb = detecteeArea.ltrb;

        float outer = detecteeArea.outer;
        float inner = detecteeArea.inner;
        float radius = detecteeArea.radius;
        float fx = 0.0f;

        if(Vector2.Distance(Vector2.zero, new Vector2(dx, dy)) > radius)
            fx = outer;
        else if(dx < 0.0f)
            fx = (outer - inner) * (1 + dx / radius) + outer;
        else
            fx = (inner - outer) * (1 - dx / radius) + outer;

        return Vector2.right * fx;
    }
}
