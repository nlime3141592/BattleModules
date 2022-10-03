using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleModule : MonoBehaviour
{
    public const int MAX_DETECT_COUNT = 32;
    private Collider2D[] m_dctCols;
    private BattleModule[] m_dctMods;

    public LTRB areaD2;
    public LTRB areaD1;

    public BattleStat stat;

    private Entity m_entity;

    [Header("Debug Options")]
    public bool checker;
    public List<BattleModule> mods;

    void Start()
    {
        m_dctCols = new Collider2D[MAX_DETECT_COUNT];
        m_dctMods = new BattleModule[MAX_DETECT_COUNT];
        m_entity = GetComponent<Entity>();
    }

    private void FixedUpdate()
    {
        if(checker)
        {
            DetectBattleModule();
            mods = new List<BattleModule>(m_dctMods);
        }
    }

    void Update()
    {
        
    }

    #region Stat Change Event Functions
    public void DetectBattleModule()
    {
        float px = transform.position.x;
        float py = transform.position.y;
        Vector2 vc = new Vector2(px + areaD2.dx, py + areaD2.dy);
        Vector2 vs = new Vector2(areaD2.sx, areaD2.sy);

        int dctColCnt = Physics2D.OverlapBoxNonAlloc(vc, vs, 0.0f, m_dctCols, 1 << LayerMask.NameToLayer("Entity"));
        int dctModCnt = FilterBattleModule(m_dctCols, dctColCnt, m_dctMods);
    }

    public void GetDamage(float damage)
    {
        if(damage < 0.0f)
            throw new ArgumentException("cannot negative argument.");

        stat.ChangeHealth(-damage);
    }

    public void Heal(float health)
    {
        if(health < 0.0f)
            throw new ArgumentException("cannot negative argument.");

        stat.ChangeHealth(health);
    }
    #endregion

    private int FilterBattleModule(Collider2D[] dctCols, int dctColCnt, BattleModule[] dctMods)
    {
        GameObject obj;
        BattleModule mod;
        int i, j;
        bool cmpFound;
        bool contains;
        int dctModCnt = 0;

        for(i = 0; i < dctColCnt; i++)
        {
            obj = dctCols[i].gameObject;
            cmpFound = obj.TryGetComponent<BattleModule>(out mod);

            if(!cmpFound)
                continue;

            j = 0;
            contains = false;

            for(j = 0; j < dctModCnt && !contains; j++)
                contains = (dctMods[j] != null && dctMods[j] == mod);

            if(!contains)
            {
                dctMods[i] = mod;
                dctModCnt++;
            }
        }

        for(i = dctModCnt; i < MAX_DETECT_COUNT; i++)
            dctMods[i] = null;

        return dctModCnt;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BattleGizmos.DrawAreaLTRB(transform.position, areaD2, Color.cyan);
        BattleGizmos.DrawAreaLTRB(transform.position, areaD1, Color.red);
    }
#endif
}
