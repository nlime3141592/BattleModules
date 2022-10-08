using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleModule : MonoBehaviour
{
    public const int MAX_DETECT_COUNT = 48;
    private Collider2D[] m_dctCols;
    private int m_dctColCnt;
    private BattleModule[] m_dctMods;
    private int m_dctModCnt;

    [Header("Module Properties")]
    public BattleStat stat;

    // Detect options
    public LTRB areaD2;
    public LTRB areaD1;
    public LayerMask targetLayer;
    public List<string> targetTags;

    public Entity entity => m_entity;
    private Entity m_entity;

    #region Unity Event Functions
    private void Start()
    {
        m_dctCols = new Collider2D[MAX_DETECT_COUNT];
        m_dctMods = new BattleModule[MAX_DETECT_COUNT];
        m_entity = GetComponent<Entity>();

        if(randomizeLTRB)
        {
            System.Random prng = new System.Random();
            areaD2.Randomize(prng, 3.0f, 5.0f);
            areaD1.Randomize(prng, 1.0f, 3.0f);
        }
    }

    private void FixedUpdate()
    {
        if(AutoCheckDie())
            return;

        if(m_entity.canDetect)
            Detect(m_dctCols, m_dctMods, m_entity.detectRange, MAX_DETECT_COUNT, out m_dctColCnt, out m_dctModCnt);

        if(m_entity.isAttack)
        {
            m_entity.isAttack = false;
            Attack();
        }

        mods = new List<BattleModule>(m_dctMods);
        cols = new List<Collider2D>(m_dctCols);

        // AutoAttack();
    }
    #endregion

    #region Automatic Executor
    public void Attack()
    {
        Detect(m_dctCols, m_dctMods, m_entity.attackRange, MAX_DETECT_COUNT, out m_dctColCnt, out m_dctModCnt);

        DebugAttackGizmo.Show(transform.position, m_entity.attackRange, Color.yellow);

        if(m_dctModCnt > 0)
        {
            float damage = stat.maxPhysicalPower; // TODO: 현재는 max로 설정했지만 후에 min과 max 사이 랜덤한 값을 불러올 수 있도록 해야 함.

            DamageEventData evdat = new DamageEventData();
            evdat.damage = damage;
            evdat.attacker = this;

            for(int i = 0; i < m_dctModCnt; i++)
                m_dctMods[i].m_Damage(evdat);

            Debug.Log("적을 찾음");
        }
        else
        {
            Debug.Log("적을 찾지 못함");
        }
    }

    private void AutoAttack()
    {
        if(m_entity.attackTimes == 0)
            m_entity.attackFrameCur = 0;
        else if(m_entity.attackTimes < 0) // NOTE: 오류 방지
            m_entity.attackTimes = 0;
        else if(m_entity.attackFrameCur > 0)
            m_entity.attackFrameCur--;
        else
        {
            int i;
            float damage = stat.maxPhysicalPower; // TODO: 현재는 max로 설정했지만 후에 min과 max 사이 랜덤한 값을 불러올 수 있도록 해야 함.

            DamageEventData evdat = new DamageEventData();
            evdat.damage = damage;
            evdat.attacker = this;

            for(i = 0; i < m_dctModCnt; i++)
                m_dctMods[i].m_Damage(evdat);

            m_entity.attackFrameCur = m_entity.attackFrame;
            m_entity.attackTimes--;
        }
    }

    private bool AutoCheckDie()
    {
        bool curDie = stat.health <= 0;

        if(!m_entity.isDie && curDie)
        {
            DieEventData evdat = new DieEventData();

            m_Die(evdat);

            m_entity.isDie = true;
        }

        return curDie;
    }
    #endregion

    #region Stat Change Event Functions
    public void Detect(Collider2D[] dctCols, BattleModule[] dctMods, LTRB ltrb, int maxCnt, out int dctColCnt, out int dctModCnt)
    {
        float px = transform.position.x;
        float py = transform.position.y;
        Vector2 vc = new Vector2(px + ltrb.dx, py + ltrb.dy);
        Vector2 vs = new Vector2(ltrb.sx, ltrb.sy);

        dctColCnt = Physics2D.OverlapBoxNonAlloc(vc, vs, 0.0f, dctCols, targetLayer);
        dctModCnt = FilterBattleModule(dctCols, dctColCnt, dctMods);
    }

    public void m_Detect()
    {
        float px = transform.position.x;
        float py = transform.position.y;
        Vector2 vc = new Vector2(px + areaD2.dx, py + areaD2.dy);
        Vector2 vs = new Vector2(areaD2.sx, areaD2.sy);

        int dctColCnt = Physics2D.OverlapBoxNonAlloc(vc, vs, 0.0f, m_dctCols, targetLayer);
        int dctModCnt = FilterBattleModule(m_dctCols, dctColCnt, m_dctMods);

        m_dctColCnt = dctColCnt;
        m_dctModCnt = dctModCnt;
    }

    private void m_Damage(DamageEventData evdat)
    {
        evdat.baseHealth = stat.health;
        stat.ChangeHealth(-evdat.damage);
        evdat.currentHealth = stat.health;
        evdat.deltaHealth = evdat.currentHealth - evdat.baseHealth;
        evdat.victim = this;

        if(m_entity != null)
            m_entity.asm_OnDamage(this, evdat);
    }

    private void m_Heal(HealEventData evdat)
    {
        evdat.baseHealth = stat.health;
        stat.ChangeHealth(evdat.healAmount);
        evdat.currentHealth = stat.health;
        evdat.deltaHealth = evdat.currentHealth - evdat.baseHealth;
        evdat.healee = this;

        if(m_entity != null)
            m_entity.asm_OnHeal(this, evdat);
    }

    private void m_Expense(ExpenseEventData evdat)
    {
        evdat.baseMana = stat.mana;
        stat.ChangeMana(-evdat.expenseAmount);
        evdat.currentMana = stat.mana;
        evdat.deltaMana = evdat.currentMana - evdat.baseMana;
        evdat.customer = this;

        if(m_entity != null)
            m_entity.asm_OnExpense(this, evdat);
    }

    private void m_Charge(ChargeEventData evdat)
    {
        evdat.baseMana = stat.mana;
        stat.ChangeMana(evdat.chargeAmount);
        evdat.currentMana = stat.mana;
        evdat.deltaMana = evdat.currentMana - evdat.baseMana;
        evdat.chargee = this;

        if(m_entity != null)
            m_entity.asm_OnCharge(this, evdat);
    }

    private void m_Die(DieEventData evdat)
    {
        if(m_entity != null)
            m_entity.asm_OnDie(this, evdat);
    }
    #endregion

    #region Sub Algorithms
    // NOTE: For Detect() Function.
    private int FilterBattleModule(Collider2D[] dctCols, int dctColCnt, BattleModule[] dctMods)
    {
        GameObject obj;
        BattleModule mod;
        int i, j;
        bool cmpFound;
        bool contains;
        int dctModCnt = 0;

        for(i = 0; i < MAX_DETECT_COUNT; i++)
        {
            dctMods[i] = null;

            if(i >= dctColCnt)
                continue;

            obj = dctCols[i].gameObject;
            cmpFound = obj.TryGetComponent<BattleModule>(out mod);

            if(!targetTags.Contains(obj.tag) || !cmpFound)
                continue;

            contains = false;

            for(j = 0; j < dctModCnt && !contains; j++)
                contains = (dctMods[j] == mod);

            if(!contains)
                dctMods[dctModCnt++] = mod;
        }

        return dctModCnt;
    }
    #endregion

    #region Predefined Exceptions
    private void CheckNegativeException(float value)
    {
        if(value < 0.0f)
            throw new ArgumentException("Value cannot be negative.");
    }
    #endregion

    #region Debug Options
    [Header("Debug Options")]
    public bool randomizeLTRB;
    public List<BattleModule> mods;
    public List<Collider2D> cols;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BattleGizmos.DrawAreaLTRB(transform.position, areaD2, Color.cyan);
        BattleGizmos.DrawAreaLTRB(transform.position, areaD1, Color.red);

        if(m_entity != null)
        {
            BattleGizmos.DrawAreaLTRB(transform.position, m_entity.detectRange, Color.green);
        }
    }
#endif
    #endregion
}