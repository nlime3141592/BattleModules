using UnityEngine;

public class DebugAttackGizmo : MonoBehaviour
{
    public int fps = 20;
    public int lfps = 0;

    private Vector2 p;
    private LTRB area;
    private Color clr;

    public static void Show(Vector2 position, LTRB ltrb, Color color)
    {
        GameObject obj = new GameObject();
        DebugAttackGizmo g = obj.AddComponent<DebugAttackGizmo>();
        obj.transform.position = position;
        g.p = position;
        g.area = ltrb;
        g.clr = color;
    }

    private void Start()
    {
        lfps = fps;
    }

    private void FixedUpdate()
    {
        if(lfps > 0)
            lfps--;
        else if(lfps == 0)
            Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        BattleGizmos.DrawAreaLTRB(p, area, clr);
    }
}