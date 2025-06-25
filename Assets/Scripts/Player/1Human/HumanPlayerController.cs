using UnityEngine;
public class HumanPlayerController : MonoBehaviour
{
    private HumanMovementComponent move;
    private HumanAnimationComponent anim;
    private HumanInputComponent input;
    private HumanAttackComponent atk;
    private StateMachine sm;
    private HumanIdleState idle;
    private float lastRoll, lastAtk;
    public float rollCooldown = 1f, attackCooldown = 1f, attackDuration = 0.7f;
    private void Awake()
    {
        move = GetComponent<HumanMovementComponent>();
        anim = GetComponent<HumanAnimationComponent>();
        input = GetComponent<HumanInputComponent>();
        atk = GetComponent<HumanAttackComponent>();
        sm = new StateMachine();
        idle = new HumanIdleState(move, anim, input, sm);
    }
    private void Start() => sm.ChangeState(idle);
    private void Update()
    {
        sm.Update(Time.deltaTime);
        move.Move(input.MoveDir, Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastRoll + rollCooldown)
        {
            sm.ChangeState(new HumanRollState(move, anim, input, sm));
            lastRoll = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= lastAtk + attackCooldown)
        {
            sm.ChangeState(new HumanAttackState(move, anim, atk, sm, attackDuration));
            lastAtk = Time.time;
        }
    }
}