using System;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int maxPhase;
    public int currentPhase;

    // state machine 정의

    protected virtual void ChangePhase()
    {

    }

    // XXXUpdate override해서 StateMachine.UpdateLogic() 및 StateMachine.UpdateInput() 실행

    // 사마귀 보스에 들어갈 상태 값
    private const int stIdle = 0;
    private const int stWalkForward = 1;
    private const int stWalkBack = 2;
    private const int stUpSlice = 3;
    private const int stTakeDown = 4;
    private const int stRotateSlice = 5;
    private const int stShout = 6;
    private const int stReadyKnife = 7;
    private const int stJumpTakeDown = 8;
    private const int stCombo00 = 9;
    private const int stCombo01 = 10;

    private bool stComboState_00;
    private bool stComboState_01;

    // GridRandom.SetWeight() 함수 사용
    private void SetWeight00()
    {
        // gprng.SetWeight(value, 3, 3, stIdle);
        // gprng.SetWeight(0, 3, 3, stWalkForward);
        // gprng.SetWeight(value, 3, 3, stWalkBack);
        // gprng.SetWeight(40, 3, 3, stUpSlice);
        // gprng.SetWeight(value, 3, 3, stTakeDown);
        // gprng.SetWeight(value, 3, 3, stRotateSlice);
        // gprng.SetWeight(value, 3, 3, stShout);
        // gprng.SetWeight(value, 3, 3, stReadyKnife);
        // gprng.SetWeight(value, 3, 3, stJumpTakeDown);
    }
}