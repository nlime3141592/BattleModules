using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public partial class _Entity : MonoBehaviour
{
    public const float MIN_PX = -5.0f;
    public const float MAX_PX = 5.0f;

    public const float MIN_PY = -5.0f;
    public const float MAX_PY = 5.0f;

    protected SpriteRenderer sprnd;

    // NOTE: BattleModule 어셈블리의 partial Entity class가 RegisterBattleModule() 함수를 Implementation합니다.
    // 고민:
    // 빌드 후, 외부에서 누가 잘못된 코드를 Implementation한 dll 파일을 접목시켜 프로그램을 re-build, re-build한 프로그램을 실행하면 프로그램이 고장날 우려가 있지 않을까?
    // ex) UnityEngine.Application.Quit() 함수를 포함하고 있는 코드를 집어넣는다던지...
    partial void RegisterBattleModule();

    protected virtual void Start()
    {
        RegisterBattleModule();

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