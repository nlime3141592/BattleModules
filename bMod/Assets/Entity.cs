using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public const float MIN_PX = -10.0f;
    public const float MAX_PX = 10.0f;

    public const float MIN_PY = -10.0f;
    public const float MAX_PY = 10.0f;

    private SpriteRenderer sprnd;

    private void Start()
    {
        sprnd = GetComponent<SpriteRenderer>();
        sprnd.color = UnityEngine.Random.ColorHSV();

        float px = GetP(sprnd.color.r, MIN_PX, MAX_PX);
        float py = GetP(sprnd.color.g, MIN_PY, MAX_PY);

        transform.position = new Vector3(px, py, -1.0f);
    }

    private float GetP(float range, float min, float max)
    {
        float dp = max - min;
        float p = min + range * dp;
        return p;
    }
}
