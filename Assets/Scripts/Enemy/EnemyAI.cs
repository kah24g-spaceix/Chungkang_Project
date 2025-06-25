using UnityEngine;

namespace EnemyAI
{
    public enum AIStateType
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Search,
        Flee,
        Dead
    }

    public enum BehaviorResult
    {
        Success,
        Failure,
        Running
    }
    public interface IAIState
    {
        AIStateType StateType { get; }
        void OnEnter(EnemyAIController controller);
        void OnUpdate(EnemyAIController controller);
        void OnExit(EnemyAIController controller);
        AIStateType CheckTransitions(EnemyAIController controller);
    }

    public abstract class BehaviorNode
    {
        public abstract BehaviorResult Execute(EnemyAIController controller);
    }

    [System.Serializable]
    public class AIBlackboard
    {
        [Header("Player Detection")]
        public Transform player;
        public Vector3 lastKnownPlayerPosition;
        public bool canSeePlayer;
        public float distanceToPlayer;
        public float lastSeenPlayerTime;

        [Header("AI State")]
        public float health;
        public float maxHealth;
        public bool isAlerted;
        public float alertLevel;

        [Header("Movement")]
        public Vector3[] patrolPoints;
        public int currentPatrolIndex;
        public Vector3 currentDestination;
        public bool hasReachedDestination;

        [Header("Combat")]
        public float lastAttackTime;
        public bool isAttacking;
        public float attackCooldown = 2f;

        public float HealthPercentage => maxHealth > 0 ? health / maxHealth : 0f;
    }
}