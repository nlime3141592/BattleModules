using System;
using UnityEngine;

public class Boss : Entity
{
    private int phaseBef = 0;
    public int currentPhase = 0;

    public int CURRENT_STATE;
    private int crst;
    public Player PLAYER;
    public Animator animator;
    public SpriteRenderer spRenderer;
    public Rigidbody2D rigid;

    // GridAI Options
    private System.Random m_prng;
    public GridPoint gp0;
    public GridPoint gp1;
    public GridPoint gp2;
    public GridPoint gp3;
    public GridPoint gp4;
    public int pulseAverage = 68;
    public int pulseRange = 37;

    // 사마귀 보스에 들어갈 상태 값
    private const int stCnt = 11; // 상태의 갯수

    private const int stIdle = 0;
    private const int stWalkForward = 1;
    private const int stWalkBack = 2;
    private const int stUpSlice = 3;
    private const int stTakeDown = 4;
    private const int stRotateSlice = 5;
    private const int stShout = 6;
    private const int stReadyKnife = 7;
    private const int stJumpTakeDown = 8;
    private const int stCombo00 = 9; // 2페이즈 진입 상태
    private const int stCombo01 = 10; // 포효 콤보

    private int b_stCombo = -1;

    // state machine 정의
    private StateMachine m_machine;
    private GridAI gai;
    public bool isEndOfAnimation;
    public int lookDir;
    public int logicFps;
    public int gx;
    public int gy;

    // 기타 필요한 변수
    public LTRB pushForceArea;

    private void UpdateLookDir()
    {   
        spRenderer.flipX = (lookDir == 1);
    }

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        spRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        m_machine = new StateMachine(stIdle);
        m_prng = new System.Random();

        m_machine.SetCallbacks(stIdle, Input_Idle, Logic_Idle, Enter_Idle, null);
        m_machine.SetCallbacks(stWalkForward, Input_WalkForward, Logic_WalkForward, Enter_WalkForward, null);
        m_machine.SetCallbacks(stWalkBack, Input_WalkBack, Logic_WalkBack, Enter_WalkBack, null);
        m_machine.SetCallbacks(stUpSlice, Input_UpSlice, Logic_UpSlice, Enter_UpSlice, null);
        m_machine.SetCallbacks(stTakeDown, Input_TakeDown, Logic_TakeDown, Enter_TakeDown, null);
        m_machine.SetCallbacks(stRotateSlice, Input_RotateSlice, Logic_RotateSlice, Enter_RotateSlice, null);
        m_machine.SetCallbacks(stShout, Input_Shout, Logic_Shout, Enter_Shout, null);
        m_machine.SetCallbacks(stReadyKnife, Input_ReadyKnife, Logic_ReadyKnife, Enter_ReadyKnife, null);
        m_machine.SetCallbacks(stJumpTakeDown, Input_JumpTakeDown, Logic_JumpTakeDown, Enter_JumpTakeDown, null);
        m_machine.SetCallbacks(stCombo00, null, null, Enter_Combo00, null);
        m_machine.SetCallbacks(stCombo01, null, null, Enter_Combo01, null);

        gai = new GridAI(m_prng, new GridPoint[]{gp0, gp1, gp2, gp3, gp4}, stCnt, pulseAverage, pulseRange);
        SetNormalWeight();
        SetWeight00();

        if(lookDir == 0)
            lookDir = 1;
    }

    private void FixedUpdate()
    {
        if(PLAYER != null)
        {
            if(currentPhase != phaseBef)
            {
                if(currentPhase == 0)
                    SetWeight00();
                else if(currentPhase == 1)
                    SetWeight01();
            }

            float dx = PLAYER.transform.position.x - transform.position.x;
            float dy = PLAYER.transform.position.y - transform.position.y;

            gai.Capture(dx * lookDir, dy);
            UpdateLookDir();

            m_machine.UpdateLogic();
        }
        else
        {
            gai.Uncapture();

            if(m_machine.state != stIdle)
                m_machine.ChangeState(stIdle);
        }

        // DetectPushArea();
    }

    private void Update()
    {
        if(PLAYER != null)
        {
            m_machine.UpdateInput();
        }

        crst = CURRENT_STATE;
        CURRENT_STATE = m_machine.state;
        if(crst != CURRENT_STATE) Debug.Log(string.Format("상태 전이: {0}->{1}", crst, CURRENT_STATE));
        animator.SetInteger("currentState", m_machine.state);

        gx = gai.m_gx;
        gy = gai.m_gy;

    }

    // XXXUpdate override해서 StateMachine.UpdateLogic() 및 StateMachine.UpdateInput() 실행

    // GridRandom.SetWeight() 함수 사용
    // NOTE: 1페이즈 AI 가중치.
    private void SetNormalWeight()
    {
        gai.SetNormalWeight(1, stIdle);
    }

    private void SetWeight00()
    {
        gai.SetGridWeight(20, 0, 3, stWalkForward);
        gai.SetGridWeight(80, 0, 3, stRotateSlice);

        gai.SetGridWeight(20, 0, 4, stWalkForward);
        gai.SetGridWeight(80, 0, 4, stRotateSlice);

        gai.SetGridWeight(20, 0, 5, stWalkForward);
        gai.SetGridWeight(80, 0, 5, stRotateSlice);

        gai.SetGridWeight(20, 1, 3, stWalkForward);
        gai.SetGridWeight(80, 1, 3, stRotateSlice);

        gai.SetGridWeight(20, 1, 4, stWalkForward);
        gai.SetGridWeight(80, 1, 4, stRotateSlice);

        gai.SetGridWeight(20, 1, 5, stWalkForward);
        gai.SetGridWeight(80, 1, 5, stRotateSlice);

        gai.SetGridWeight(20, 2, 3, stWalkForward);
        gai.SetGridWeight(80, 2, 3, stRotateSlice);

        gai.SetGridWeight(20, 2, 4, stWalkForward);
        gai.SetGridWeight(80, 2, 4, stRotateSlice);

        gai.SetGridWeight(20, 2, 5, stWalkForward);
        gai.SetGridWeight(80, 2, 5, stRotateSlice);

        gai.SetGridWeight(50, 3, 3, stUpSlice);
        gai.SetGridWeight(30, 3, 3, stTakeDown);
        gai.SetGridWeight(20, 3, 3, stWalkBack);

        gai.SetGridWeight(50, 4, 3, stTakeDown);
        gai.SetGridWeight(20, 4, 3, stUpSlice);
        gai.SetGridWeight(20, 4, 3, stWalkBack);
        gai.SetGridWeight(10, 4, 3, stWalkForward);

        gai.SetGridWeight(70, 5, 3, stWalkForward);
        gai.SetGridWeight(30, 5, 3, stWalkBack);

        gai.SetGridWeight(70, 3, 4, stUpSlice);
        gai.SetGridWeight(20, 3, 4, stWalkBack);
        gai.SetGridWeight(10, 3, 4, stTakeDown);

        gai.SetGridWeight(40, 4, 4, stTakeDown);
        gai.SetGridWeight(30, 4, 4, stUpSlice);
        gai.SetGridWeight(20, 4, 4, stWalkBack);
        gai.SetGridWeight(10, 4, 4, stWalkForward);

        gai.SetGridWeight(70, 5, 4, stWalkForward);
        gai.SetGridWeight(30, 5, 4, stWalkBack);

        gai.SetGridWeight(40, 3, 5, stUpSlice);
        gai.SetGridWeight(60, 3, 5, stWalkBack);

        gai.SetGridWeight(50, 3, 5, stUpSlice);
        gai.SetGridWeight(50, 3, 5, stWalkBack);

        gai.SetGridWeight(70, 5, 5, stWalkForward);
        gai.SetGridWeight(30, 5, 5, stWalkBack);
    }

    // NOTE: 2페이즈 AI 가중치.
    private void SetWeight01()
    {
        // 0
        gai.SetGridWeight(20, 0, 3, stWalkForward);
        gai.SetGridWeight(80, 0, 3, stRotateSlice);

        gai.SetGridWeight(20, 0, 4, stWalkForward);
        gai.SetGridWeight(80, 0, 4, stRotateSlice);

        gai.SetGridWeight(20, 0, 5, stWalkForward);
        gai.SetGridWeight(80, 0, 5, stRotateSlice);

        gai.SetGridWeight(20, 1, 3, stWalkForward);
        gai.SetGridWeight(80, 1, 3, stRotateSlice);

        gai.SetGridWeight(20, 1, 4, stWalkForward);
        gai.SetGridWeight(80, 1, 4, stRotateSlice);

        gai.SetGridWeight(20, 1, 5, stWalkForward);
        gai.SetGridWeight(80, 1, 5, stRotateSlice);

        gai.SetGridWeight(20, 2, 3, stWalkForward);
        gai.SetGridWeight(80, 2, 3, stRotateSlice);

        gai.SetGridWeight(20, 2, 4, stWalkForward);
        gai.SetGridWeight(80, 2, 4, stRotateSlice);

        gai.SetGridWeight(20, 2, 5, stWalkForward);
        gai.SetGridWeight(80, 2, 5, stRotateSlice);

        // 1
        gai.SetGridWeight(40, 3, 3, stUpSlice);
        gai.SetGridWeight(20, 3, 3, stCombo01);
        gai.SetGridWeight(30, 3, 3, stTakeDown);
        gai.SetGridWeight(10, 3, 3, stWalkBack);

        // 2
        gai.SetGridWeight(40, 4, 3, stTakeDown);
        gai.SetGridWeight(20, 4, 3, stCombo01);
        gai.SetGridWeight(20, 4, 3, stUpSlice);
        gai.SetGridWeight(10, 4, 3, stWalkBack);
        gai.SetGridWeight(10, 4, 3, stWalkForward);

        // 3
        gai.SetGridWeight(70, 5, 3, stJumpTakeDown);
        gai.SetGridWeight(30, 5, 3, stWalkForward);

        // 4
        gai.SetGridWeight(50, 3, 4, stUpSlice);
        gai.SetGridWeight(20, 3, 4, stCombo01);
        gai.SetGridWeight(20, 3, 4, stWalkBack);
        gai.SetGridWeight(10, 3, 4, stTakeDown);

        // 5
        gai.SetGridWeight(30, 4, 4, stTakeDown);
        gai.SetGridWeight(30, 4, 4, stUpSlice);
        gai.SetGridWeight(20, 4, 4, stCombo01);
        gai.SetGridWeight(10, 4, 4, stWalkBack);
        gai.SetGridWeight(10, 4, 4, stWalkForward);

        // 6
        gai.SetGridWeight(70, 5, 4, stJumpTakeDown);
        gai.SetGridWeight(30, 5, 4, stWalkForward);

        // 7
        gai.SetGridWeight(60, 3, 5, stWalkBack);
        gai.SetGridWeight(40, 3, 5, stUpSlice);

        // 8
        gai.SetGridWeight(50, 3, 5, stUpSlice);
        gai.SetGridWeight(50, 3, 5, stWalkBack);

        // 9
        gai.SetGridWeight(70, 5, 5, stJumpTakeDown);
        gai.SetGridWeight(30, 5, 5, stWalkForward);
    }

    #region External Functions
    public void OnEndAnimation()
    {
        isEndOfAnimation = true;
    }

    public void OnAttack()
    {
        isAttack = true;
    }
    #endregion

    #region Implement State; stIdle
    private void Enter_Idle()
    {
        // TODO:
        // 초기 Enter를 감지할 수 있는 bool 변수 하나
        // 초기 Enter일 경우 logicFps를 길게 설정하기.
        logicFps = (int)m_prng.RangeNextDouble(pulseAverage, pulseRange);
    }

    private void Input_Idle()
    {
        if(logicFps == 0)
        {
            if(gai.isCaptured)
            {
                int st = gai.NEXT_CAPTURE_STATE();
                m_machine.ChangeState(st < 0 ? 0 : st);
            }
            else
                m_machine.ChangeState(gai.NEXT_UNCAPTURE_STATE());
        }
    }

    private void Logic_Idle()
    {
        if(logicFps > 0)
        {
            logicFps--;
            rigid.velocity = Vector2.zero;
        }
    }
    #endregion

    #region Implement State; stWalkForward
    private void Enter_WalkForward()
    {
        logicFps = (int)m_prng.RangeNextDouble(60, 10);

        if(gai.m_gx < 3) // NOTE: 후면에서 플레이어를 감지해서 앞으로 걷기 상태로 진입한 경우.
            lookDir = -lookDir;
    }

    private void Input_WalkForward()
    {
        if(logicFps == 0)
            m_machine.ChangeState(stIdle);
    }

    private void Logic_WalkForward()
    {
        if(logicFps > 0)
        {
            logicFps--;

            // transform.Translate(Vector3.right * GetMoveSpeed() * lookDir * Time.fixedDeltaTime);
            rigid.velocity = Vector3.right * GetMoveSpeed() * lookDir;
        }
    }
    #endregion

    #region Implement State; stWalkBack
    private void Enter_WalkBack()
    {
        logicFps = (int)m_prng.RangeNextDouble(45, 10);
    }

    private void Input_WalkBack()
    {
        if(logicFps == 0)
            m_machine.ChangeState(stIdle);
    }

    private void Logic_WalkBack()
    {
        if(logicFps > 0)
        {
            logicFps--;

            // transform.Translate(Vector3.right * GetMoveSpeed() * -lookDir * Time.fixedDeltaTime);
            rigid.velocity = Vector3.right * GetMoveSpeed() * -lookDir;
        }
    }
    #endregion

    #region Implement State; stUpSlice
    private void Enter_UpSlice()
    {
        isAttack = false;
        isEndOfAnimation = false;

        SetAttackRange(0.5f, 6.0f, 2.5f, 0.25f, lookDir);
    }

    private void Input_UpSlice()
    {
        if(isEndOfAnimation)
        {
            if(b_stCombo == 1)
            {
                if(gai.m_gx < 3) // 후면
                    m_machine.ChangeState(stRotateSlice);
                else // 전면
                    m_machine.ChangeState(stTakeDown);
            }
            else
                m_machine.ChangeState(stIdle);
        }
    }

    private void Logic_UpSlice()
    {
        rigid.velocity = Vector2.zero;
    }
    #endregion

    #region Implement State; stTakeDown
    private void Enter_TakeDown()
    {
        if(b_stCombo == 1)
            b_stCombo = -1;

        isAttack = false;
        isEndOfAnimation = false;

        SetAttackRange(0.5f, 6.0f, 6.0f, 0.25f, lookDir);
    }

    private void Input_TakeDown()
    {
        if(isEndOfAnimation)
            m_machine.ChangeState(stIdle);
    }

    private void Logic_TakeDown()
    {
        rigid.velocity = Vector2.zero;
    }
    #endregion

    #region Implement State; stRotateSlice
    private void Enter_RotateSlice()
    {
        if(b_stCombo == 1)
            b_stCombo = -1;

        isAttack = false;
        isEndOfAnimation = false;
        lookDir = -lookDir;
        SetAttackRange(0.5f, 9.0f, 6.0f, 0.25f, lookDir);
    }

    private void Input_RotateSlice()
    {
        if(isEndOfAnimation)
            m_machine.ChangeState(stIdle);
    }

    private void Logic_RotateSlice()
    {
        rigid.velocity = Vector2.zero;
    }
    #endregion

    #region Implement State; stShout
    private void Enter_Shout()
    {
        isEndOfAnimation = false;

        // NOTE: 포효 콤보에서는 플레이어에게 기절 상태를 제공하면 안되므로, 조건문 처리 해야 함.
        if(b_stCombo != 1)
        {

        }
    }

    private void Input_Shout()
    {
        if(isEndOfAnimation)
        {
            if(b_stCombo == 0)
                m_machine.ChangeState(stReadyKnife);
            else if(b_stCombo == 1)
                m_machine.ChangeState(stUpSlice);
            else
                m_machine.ChangeState(stIdle);
        }
    }

    private void Logic_Shout()
    {
        rigid.velocity = Vector2.zero;
    }
    #endregion

    #region Implement State; stReadyKnife
    private void Enter_ReadyKnife()
    {
        if(b_stCombo == 0)
            b_stCombo = -1;

        isEndOfAnimation = false;
    }

    private void Input_ReadyKnife()
    {
        if(isEndOfAnimation)
            m_machine.ChangeState(stIdle);
    }

    private void Logic_ReadyKnife()
    {
        rigid.velocity = Vector2.zero;
    }
    #endregion

    #region Implement State; stJumpTakeDown
    private void Enter_JumpTakeDown()
    {
        isAttack = false;
        isEndOfAnimation = false;

        SetAttackRange(0.5f, 6.0f, 9.0f, 0.25f, lookDir);
    }

    private void Input_JumpTakeDown()
    {
        if(isEndOfAnimation)
            m_machine.ChangeState(stIdle);
    }

    private void Logic_JumpTakeDown()
    {
        rigid.velocity = Vector2.zero;
    }
    #endregion

    #region Implement State; stCombo00
    private void Enter_Combo00()
    {
        b_stCombo = 0;
        m_machine.ChangeState(stShout);
    }
    #endregion

    #region Implement State; stCombo01
    private void Enter_Combo01()
    {
        b_stCombo = 1;
        m_machine.ChangeState(stShout);
    }
    #endregion

    private float GetMoveSpeed()
    {
        return 3.0f;
    }
/*
    public LayerMask tLayer;
    public float pushWeight = 1.2f;
    public void DetectPushArea()
    {
        Vector3 p = transform.position + new Vector3(pushForceArea.dx, pushForceArea.dy, 0.0f);
        Vector2 s = new Vector2(pushForceArea.sx, pushForceArea.sy);
        Collider2D[] cols = Physics2D.OverlapBoxAll(p, s, 0.0f, tLayer);

        for(int i = 0; i < cols.Length; i++)
        {
            if(cols[i].gameObject == this.gameObject)
                continue;

            Entity e;

            if(cols[i].gameObject.TryGetComponent<Entity>(out e))
            {
                float dx = e.transform.position.x - transform.position.x;
                float dy = e.transform.position.y - transform.position.y;
                float scala = GetPushForce(dx, dy, pushWeight);

                e.pushScala.x = scala;

                if(scala == 0.0f)
                    e.pushDir = 0;
                else if(dx < 0.0f)
                    e.pushDir = -1;
                else
                    e.pushDir = 1;
            }
        }
    }

    private float GetPushForce(float dx, float dy, float weight)
    {
        float scala = 0.0f;

        if( dy < -pushForceArea.b || dy > pushForceArea.t || dx < -pushForceArea.l || dx > pushForceArea.r)
            scala = 0.0f;
        else if(dx < 0.0f)
            scala = weight * (1 + dx / pushForceArea.l);
        else
            scala = weight * (1 - dx / pushForceArea.r);

        scala = Mathf.Sqrt(scala);
        if(scala < 0.0f) scala = 0.0f;
        else if(scala > 1.0f) scala = 1.0f;

        return scala;
    }

    protected void OnDrawGizmos()
    {
        pushForceArea.DrawGizmo(transform, Color.red);
    }
*/
}