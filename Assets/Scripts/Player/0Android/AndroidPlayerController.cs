using UnityEngine;
public class AndroidPlayerController : MonoBehaviour
{
    private AndroidMovementComponent move;
    private AndroidAnimationComponent anim;
    private AndroidInputComponent input;
    private AndroidAttackComponent atk;
    private StateMachine sm;

    private AndroidIdleState idleState;

    private float currentRoll, currentAtk = 0.0f;
    public float rollCooldown = 1f;
    public float attackCooldown = 4f;
    private void Awake()
    {
        move = GetComponent<AndroidMovementComponent>();
        anim = GetComponent<AndroidAnimationComponent>();
        input = GetComponent<AndroidInputComponent>();
        atk = GetComponent<AndroidAttackComponent>();
        sm = new StateMachine();

        idleState = new AndroidIdleState(move, anim, input, sm);
    }
    private void Start()
    {
        sm.ChangeState(idleState);
    }
    private void Update()
    {
        sm.Update(Time.deltaTime);
        move.Move(input.MoveDir, Time.deltaTime);
        if (currentRoll > 0f) currentRoll -= Time.deltaTime;
        if (currentAtk > 0f) currentAtk -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && currentRoll <= 0f)
        {
            sm.ChangeState(new AndroidRollState(move, anim, input, sm));
            currentRoll = rollCooldown;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && currentAtk <= 0f)
        {
            sm.ChangeState(new AndroidAttackState(move, anim, input, atk, sm));
            currentAtk = attackCooldown;
        }
    }
}