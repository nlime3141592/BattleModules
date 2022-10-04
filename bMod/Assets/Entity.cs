using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    public virtual void OnDamage(BattleModule pubModule, DamageEventData evdat)
    {
        StringBuilder msg = new StringBuilder();

        string aname = evdat.attacker.entity.name;
        string vname = evdat.victim.entity.name;
        float bh = evdat.baseHealth;
        float ch = evdat.currentHealth;
        float dh = evdat.deltaHealth;

        msg.AppendFormat("messager: {0}\n", this.gameObject.name);
        msg.AppendFormat("attacker: {0}\n", aname);
        msg.AppendFormat("victim: {0}\n", vname);
        msg.AppendFormat("health: {0}->{1} (dh={2})\n", bh, ch, dh);
        Debug.Log(msg.ToString());
    }

    public virtual void OnDie(BattleModule pubModule, DieEventData evdat)
    {
        StringBuilder msg = new StringBuilder();

        string modname = pubModule.entity.name;

        msg.AppendFormat("{0} has been Dead.\n", modname);
        Debug.Log(msg.ToString());
    }
}
