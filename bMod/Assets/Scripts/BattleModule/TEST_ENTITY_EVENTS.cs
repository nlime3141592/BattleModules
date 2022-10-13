using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class _Entity
{
    #region Event Handlers
    public BattleEventHandler<DamageEventData> onDamage;
    public BattleEventHandler<HealEventData> onHeal;
    public BattleEventHandler<ExpenseEventData> onExpense;
    public BattleEventHandler<ChargeEventData> onCharge;
    public BattleEventHandler<DieEventData> onDie;
    #endregion

    // TODO: 테스트가 끝나면 Intermediate Datas region의 접근 제한자를 public에서 protected internal로 교체하기.
    #region Intermediate Datas
    public bool canDetect;
    public LTRB detectRange;

    public int attackFrame;
    public int attackFrameCur;
    public int attackTimes;

    public bool isDie;

    public LTRB attackRange;
    public bool isAttack;
    #endregion

    #region Event Publishers (call by BattleModule)
    internal void asm_OnDamage(BattleModule pubModule, DamageEventData evdat) { if(onDamage != null) onDamage(pubModule, evdat); }
    internal void asm_OnHeal(BattleModule pubModule, HealEventData evdat) { if(onHeal != null) onHeal(pubModule, evdat); }
    internal void asm_OnExpense(BattleModule pubModule, ExpenseEventData evdat) { if(onExpense != null) onExpense(pubModule, evdat); }
    internal void asm_OnCharge(BattleModule pubModule, ChargeEventData evdat) { if(onCharge != null) onCharge(pubModule, evdat); }
    internal void asm_OnDie(BattleModule pubModule, DieEventData evdat) { if(onDie != null) onDie(pubModule, evdat); }
    #endregion

    #region Event Receivers
    protected virtual void OnDamage(BattleModule pubModule, DamageEventData evdat) {}
    protected virtual void OnHeal(BattleModule pubModule, HealEventData evdat) {}
    protected virtual void OnExpense(BattleModule pubModule, ExpenseEventData evdat) {}
    protected virtual void OnCharge(BattleModule pubModule, ChargeEventData evdat) {}
    protected virtual void OnDie(BattleModule pubModule, DieEventData evdat) {}
    #endregion

    #region Unity Event Functions
    partial void RegisterBattleModule()
    {
        onDamage = OnDamage;
        onHeal = OnHeal;
        onExpense = OnExpense;
        onCharge = OnCharge;
        onDie = OnDie;
    }
    #endregion

    #region Utilities
    // NOTE: 엔티티가 오른쪽을 바라보고 있을 때 기준으로 ltrb 값을 대입하세요.
    protected void SetAttackRange(float l, float t, float r, float b, int lookDir)
    {
        if(lookDir == -1)
        {
            attackRange.l = r;
            attackRange.r = l;
        }
        else
        {
            attackRange.l = l;
            attackRange.r = r;
        }

        attackRange.t = t;
        attackRange.b = b;
    }
    #endregion
}