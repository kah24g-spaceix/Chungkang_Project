using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class PatrolTask : BlackboardTask
{
    public PatrolTask(Blackboard blackboard, BlackboardConfig config = null) : base(blackboard) 
    {
        if (config != null)
        {
            // Config에서 필수 키들을 검증
            foreach (var key in GetRequiredKeys())
            {
                if (config.IsKeyRequired(key) && !blackboard.HasValue(key))
                {
                    Debug.LogError($"Required key {key} is missing from blackboard");
                }
            }
        }
    }

    public override List<BlackboardKey> GetRequiredKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.Agent,
            BlackboardKey.Config,
            BlackboardKey.Transform,
            BlackboardKey.SpawnPosition
        };
    }

    public override List<BlackboardKey> GetOptionalKeys()
    {
        return new List<BlackboardKey>
        {
            BlackboardKey.Animator,
            BlackboardKey.PatrolPoints
        };
    }

    public override void InitializeBlackboardData(Blackboard blackboard)
    {
        if (!blackboard.HasValue(BlackboardKey.CurrentPatrolTarget))
            blackboard.SetValue(BlackboardKey.CurrentPatrolTarget, Vector3.zero);

        if (!blackboard.HasValue(BlackboardKey.CurrentPatrolIndex))
            blackboard.SetValue(BlackboardKey.CurrentPatrolIndex, 0);

        if (!blackboard.HasValue(BlackboardKey.PatrolWaitTimer))
            blackboard.SetValue(BlackboardKey.PatrolWaitTimer, 0f);
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
        var selfTrans = blackboard.GetValue<Transform>(BlackboardKey.Transform);
        var animator = blackboard.GetValue<Animator>(BlackboardKey.Animator);
        var patrolPoints = blackboard.GetValue<Transform[]>(BlackboardKey.PatrolPoints);
        
        bool isWaiting = blackboard.GetValue<bool>(BlackboardKey.IsPatrolWaiting);
        float waitTimer = blackboard.GetValue<float>(BlackboardKey.PatrolWaitTimer);
        Vector3 target = blackboard.GetValue<Vector3>(BlackboardKey.CurrentPatrolTarget);
        Vector3 spawnPos = blackboard.GetValue<Vector3>(BlackboardKey.SpawnPosition);
        // 목표 지점 설정
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            int index = blackboard.GetValue<int>(BlackboardKey.CurrentPatrolIndex);

            if (!isWaiting)
            {
                target = patrolPoints[index].position;
                agent.isStopped = false;
                agent.speed = config.walkSpeed;
                agent.SetDestination(target);

                // 도착 판정
                if (!agent.pathPending && agent.remainingDistance < config.patrolArrivalDistance)
                {
                    agent.isStopped = true;
                    blackboard.SetValue(BlackboardKey.IsPatrolWaiting, true);
                    blackboard.SetValue(BlackboardKey.PatrolWaitTimer, 0f);
                }
            }
            else
            {
                waitTimer += Time.deltaTime;
                blackboard.SetValue(BlackboardKey.PatrolWaitTimer, waitTimer);
                if (waitTimer >= config.patrolWaitTime)
                {
                    blackboard.SetValue(BlackboardKey.IsPatrolWaiting, false);
                    index = (index + 1) % patrolPoints.Length;
                    blackboard.SetValue(BlackboardKey.CurrentPatrolIndex, index);
                }
            }
        }
        else
        {
            // 랜덤 배회
            if (!isWaiting)
            {
                if (target == Vector3.zero || (!agent.pathPending && agent.remainingDistance < config.patrolArrivalDistance))
                {
                    Vector3 randDir = Random.insideUnitSphere * config.patrolRadius;
                    randDir += spawnPos;
                    randDir.y = spawnPos.y;

                    if (NavMesh.SamplePosition(randDir, out NavMeshHit hit, config.patrolRadius, NavMesh.AllAreas))
                        target = hit.position;
                    else
                        target = spawnPos;

                    agent.isStopped = false;
                    agent.speed = config.walkSpeed;
                    agent.SetDestination(target);
                }
                else if (!agent.pathPending && agent.remainingDistance < config.patrolArrivalDistance)
                {
                    agent.isStopped = true;
                    blackboard.SetValue(BlackboardKey.IsPatrolWaiting, true);
                    blackboard.SetValue(BlackboardKey.PatrolWaitTimer, 0f);
                }
            }
            else
            {
                waitTimer += Time.deltaTime;
                blackboard.SetValue(BlackboardKey.PatrolWaitTimer, waitTimer);
                if (waitTimer >= config.patrolWaitTime)
                {
                    blackboard.SetValue(BlackboardKey.IsPatrolWaiting, false);
                    blackboard.SetValue(BlackboardKey.PatrolWaitTimer, 0f);
                    target = Vector3.zero;
                }
            }
        }

        // 블랙보드에 현재 목표 저장
        blackboard.SetValue(BlackboardKey.CurrentPatrolTarget, target);

        // 애니메이션 업데이트
        if (animator != null)
        {
            float speed = agent.desiredVelocity.magnitude;
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsPatrolling", !agent.isStopped);
        }

        state = NodeState.Running;
        return state;
    }
}
