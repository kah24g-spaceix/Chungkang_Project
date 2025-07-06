using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class AttackTask : BlackboardTask
{
    public AttackTask(Blackboard blackboard) : base(blackboard) { }

    public override List<BlackboardKey> GetRequiredKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.Agent,
            BlackboardKey.Config,
            BlackboardKey.Transform,
            BlackboardKey.Player
        };
    }

    public override List<BlackboardKey> GetOptionalKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.Animator
        };
    }

    public override void InitializeBlackboardData(Blackboard blackboard)
    {
        if (!blackboard.HasValue(BlackboardKey.LastAttackTime))
            blackboard.SetValue(BlackboardKey.LastAttackTime, 0f);
        
        if (!blackboard.HasValue(BlackboardKey.IsAttacking))
            blackboard.SetValue(BlackboardKey.IsAttacking, false);
        
        if (!blackboard.HasValue(BlackboardKey.AttackTimer))
            blackboard.SetValue(BlackboardKey.AttackTimer, 0f);
        
        if (!blackboard.HasValue(BlackboardKey.DamageApplied))
            blackboard.SetValue(BlackboardKey.DamageApplied, false);
    }

    public override NodeState Evaluate()
    {
        if (!ValidateRequiredData())
        {
            state = NodeState.Failure;
            return state;
        }

        var agent = blackboard.GetValue<NavMeshAgent>(BlackboardKey.Agent);
        var config = blackboard.GetValue<EnemyConfig>(BlackboardKey.Config);
        var animator = blackboard.GetValue<Animator>(BlackboardKey.Animator);
        var selfTrans = blackboard.GetValue<Transform>(BlackboardKey.Transform);
        var player = blackboard.GetValue<Transform>(BlackboardKey.Player);

        agent.SetDestination(selfTrans.position);

        Vector3 dir = (player.position - selfTrans.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            selfTrans.rotation = Quaternion.Slerp(
                selfTrans.rotation,
                Quaternion.LookRotation(dir),
                config.rotationSpeed * Time.deltaTime
            );
        }

        float lastAttack = blackboard.GetValue<float>(BlackboardKey.LastAttackTime);
        if (Time.time - lastAttack < config.attackCooldown)
        {
            state = NodeState.Failure;
            return state;
        }

        bool isAttacking = blackboard.GetValue<bool>(BlackboardKey.IsAttacking);
        float attackTimer = blackboard.GetValue<float>(BlackboardKey.AttackTimer);
        bool damageApplied = blackboard.GetValue<bool>(BlackboardKey.DamageApplied);

        if (!isAttacking)
        {
            if (animator != null)
                animator.SetTrigger("Attack");
            
            blackboard.SetValue(BlackboardKey.LastAttackTime, Time.time);
            blackboard.SetValue(BlackboardKey.IsAttacking, true);
            blackboard.SetValue(BlackboardKey.AttackTimer, 0f);
            blackboard.SetValue(BlackboardKey.DamageApplied, false);

            state = NodeState.Running;
            return state;
        }

        attackTimer += Time.deltaTime;
        blackboard.SetValue(BlackboardKey.AttackTimer, attackTimer);

        if (!damageApplied && attackTimer >= config.attackDamageDelay)
        {
            var baseStats = player.GetComponent<BasePlayerStats>();
            var androidStats = player.GetComponent<AndroidPlayerStats>();
            int damage = Mathf.RoundToInt(config.attackDamage);

            if (baseStats != null)
                baseStats.TakeDamage(damage);
            else if (androidStats != null)
                androidStats.TakeDamage(damage);

            Debug.Log($"Enemy dealt {damage} damage to player.");
            blackboard.SetValue(BlackboardKey.DamageApplied, true);
        }

        if (attackTimer >= config.attackDuration)
        {
            blackboard.SetValue(BlackboardKey.IsAttacking, false);
            state = NodeState.Success;
            return state;
        }

        state = NodeState.Running;
        return state;
    }
}
