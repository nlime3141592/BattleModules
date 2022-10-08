using UnityEngine;

public class Player : Entity
{
    public float speed = 3.0f;
    private float xInput;
    private float yInput;

    protected override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        transform.Translate(new Vector3(xInput, yInput, 0.0f) * speed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
    }

    protected override void OnDamage(BattleModule pubModule, DamageEventData evdat)
    {
        Debug.Log(string.Format("플레이어가 한 대 맞았다! (체력: {0}->{1})", evdat.baseHealth, evdat.currentHealth));
    }
}