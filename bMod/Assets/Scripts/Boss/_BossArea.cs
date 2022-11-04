using UnityEngine;

public class _BossArea : MonoBehaviour
{
    public AreaMode mode;

    public enum AreaMode
    {
        Box, Circle
    }

    // Mode: Box
    public float box_l;
    public float box_t;
    public float box_r;
    public float box_b;

    // Mode: Circle
    public float center_dx;
    public float center_dy;
    public float radius;

    private float GetPushForce_Box(float dx, float dy, float weight)
    {
        float scala = 0.0f;

        if( dy < box_b || dy > box_t || dx < box_l || dx > box_r)
            scala = 0.0f;
        else if(dx < 0.0f)
            scala = weight * (1 + dx / box_l);
        else
            scala = weight * (1 - dx / box_r);

        scala = Mathf.Sqrt(scala);
        if(scala < 0.0f) scala = 0.0f;
        else if(scala > 1.0f) scala = 1.0f;

        return scala;
    }

    private float GetPushForce_Circle(float dx, float dy, float weight)
    {
        float scala = 0.0f;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        scala = distance / radius;
        scala *= weight;

        if(scala < 0.0f) scala = 0.0f;
        else if(scala > 1.0f) scala = 1.0f;

        return scala;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        switch(mode)
        {
            case AreaMode.Box:
                Gizmos.DrawWireCube(transform.position + new Vector3((box_r - box_l) * 0.5f, (box_t - box_b) * 0.5f, 0.0f), new Vector3(box_r + box_l, box_t + box_b));
                break;

            case AreaMode.Circle:
                Gizmos.DrawWireSphere(transform.position + new Vector3(center_dx, center_dy, 0.0f), radius);
                break;
        }
    }
}