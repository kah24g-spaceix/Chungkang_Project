using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class PatrolTask : BehaviorNode
{
    private Blackboard blackboard;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Vector3 currentTarget;
    private Vector3 spawnPosition;
    private bool hasSpawnPosition = false;
    
    public PatrolTask(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    
    public override NodeState Evaluate()
    {
        Transform[] patrolPoints = blackboard.GetValue<Transform[]>("patrolPoints");
        NavMeshAgent agent = blackboard.GetValue<NavMeshAgent>("agent");
        EnemyConfig config = blackboard.GetValue<EnemyConfig>("config");
        Transform transform = blackboard.GetValue<Transform>("transform");
        Animator animator = blackboard.GetValue<Animator>("animator");
        
        if (agent == null || config == null || transform == null)
        {
            state = NodeState.Failure;
            return state;
        }
        
        // 스폰 위치 저장 (한 번만)
        if (!hasSpawnPosition)
        {
            spawnPosition = transform.position;
            hasSpawnPosition = true;
            blackboard.SetValue("spawnPosition", spawnPosition);
        }
        
        // 패트롤 포인트가 있는 경우 - 기존 로직
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            return PatrolWithPoints(patrolPoints, agent, config, animator);
        }
        // 패트롤 포인트가 없는 경우 - 랜덤 배회
        else
        {
            return RandomWander(agent, config, transform, animator);
        }
    }
    
    private NodeState PatrolWithPoints(Transform[] patrolPoints, NavMeshAgent agent, EnemyConfig config, Animator animator)
    {
        int currentIndex = blackboard.GetValue<int>("currentPatrolIndex");
        
        if (!isWaiting)
        {
            agent.speed = config.walkSpeed;
            agent.SetDestination(patrolPoints[currentIndex].position);
            
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                isWaiting = true;
                waitTimer = 0f;
            }
        }
        else
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= config.patrolWaitTime)
            {
                isWaiting = false;
                currentIndex = (currentIndex + 1) % patrolPoints.Length;
                blackboard.SetValue("currentPatrolIndex", currentIndex);
            }
        }
        
        // 애니메이션 업데이트
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("IsPatrolling", true);
        }
        
        state = NodeState.Running;
        return state;
    }
    
    private NodeState RandomWander(NavMeshAgent agent, EnemyConfig config, Transform transform, Animator animator)
    {
        if (!isWaiting)
        {
            // 현재 목표가 없거나 도착했으면 새로운 목표 설정
            if (currentTarget == Vector3.zero || (!agent.pathPending && agent.remainingDistance < 1f))
            {
                currentTarget = GetRandomWanderPoint(spawnPosition, config.patrolRadius);
                agent.speed = config.walkSpeed;
                agent.SetDestination(currentTarget);
            }
        }
        else
        {
            // 목표에 도착하면 잠깐 대기
            waitTimer += Time.deltaTime;
            if (waitTimer >= config.patrolWaitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                currentTarget = Vector3.zero; // 새로운 목표를 위해 리셋
            }
        }
        
        // 목표에 도착했는지 확인
        if (!isWaiting && !agent.pathPending && agent.remainingDistance < 1f)
        {
            isWaiting = true;
            waitTimer = 0f;
        }
        
        // 애니메이션 업데이트
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("IsPatrolling", true);
        }
        
        state = NodeState.Running;
        return state;
    }
    
    private Vector3 GetRandomWanderPoint(Vector3 center, float radius)
    {
        // 중심점 주변에서 랜덤한 점 생성
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;
        randomDirection.y = center.y; // Y축 고정
        
        // NavMesh 위의 유효한 위치 찾기
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        // 유효한 위치를 찾지 못하면 원래 스폰 위치 반환
        return center;
    }
}
