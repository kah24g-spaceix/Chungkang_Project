using UnityEngine;

public class AndroidAttackState : IState
{
    private readonly IPlayerMovement move;
    private readonly IPlayerAnimation anim;
    private readonly IPlayerInput input;
    private readonly IPlayerAttack attack;
    private readonly AndroidAttackComponent attackComponent;
    private readonly StateMachine sm;

    public AndroidAttackState(
        IPlayerMovement m,
        IPlayerAnimation a,
        IPlayerInput i,
        IPlayerAttack atkC,
        StateMachine s)
    {
        move = m;
        anim = a;
        input = i;
        attack = atkC;
        sm = s;

        attackComponent = atkC as AndroidAttackComponent;
        if (attackComponent == null)
        {
            throw new System.ArgumentNullException(nameof(attackComponent), "AttackComponent must be AndroidAttackComponent");
        }
    }

    public void Enter()
    {
        Debug.Log("AndroidAttackState Enter - Starting Attack");

        attackComponent.Attack();
        Debug.Log("Attack initiated from AndroidAttackState");
    }

    public void Execute(float dt)
    {
        HandleAttackInput();

        if (!attackComponent.IsAttacking)
        {
            Debug.Log("Attack finished, returning to Idle");
            sm.ChangeState(new AndroidIdleState(move, anim, input, attack, sm));
        }
    }

    public void Exit()
    {
        Debug.Log("AndroidAttackState Exit");
    }

    private void HandleAttackInput()
    {
        if (input.IsAttackPressed)
        {
            Debug.Log($"Attack input in state - CanCombo: {attackComponent.CanCombo}");

            if (attackComponent.CanCombo)
            {
                attackComponent.TryComboAttack();
                Debug.Log("Combo attack triggered");
            }
        }
    }
}
