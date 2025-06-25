using UnityEngine;
using EnemyAI.States;

namespace EnemyAI
{
    [RequireComponent(typeof(AIMovement))]
    [RequireComponent(typeof(AISensorSystem))]
    public class EnemyAIController : MonoBehaviour
    {
        [Header("AI Configuration")]
        public AIStateType initialState = AIStateType.Patrol;
        public AIBlackboard blackboard = new AIBlackboard();

        [Header("Patrol Settings")]
        public Transform[] patrolPoints;
        public float patrolWaitTime = 2f;

        [Header("Combat Settings")]
        public float attackRange = 2f;
        public int attackDamage = 10;

        private AIStateMachine stateMachine;
        private AIMovement movement;
        private AISensorSystem sensorSystem;
        private Health healthComponent;

        public AIBlackboard Blackboard => blackboard;
        public AIMovement Movement => movement;
        public AISensorSystem SensorSystem => sensorSystem;
        public AIStateMachine StateMachine => stateMachine;

        void Awake()
        {
            movement = GetComponent<AIMovement>();
            sensorSystem = GetComponent<AISensorSystem>();
            healthComponent = GetComponent<Health>();
            
            InitializeBlackboard();
            InitializeStateMachine();
        }

        void Start()
        {
            stateMachine.Start(initialState);
        }

        void Update()
        {
            if (blackboard.health <= 0) return;

            sensorSystem.UpdateSensors();
            stateMachine.Update();
        }

        void InitializeBlackboard()
        {
            blackboard.maxHealth = healthComponent ? healthComponent.MaxHealth : 100f;
            blackboard.health = blackboard.maxHealth;
            blackboard.attackCooldown = 2f;

            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                blackboard.patrolPoints = new Vector3[patrolPoints.Length];
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    blackboard.patrolPoints[i] = patrolPoints[i].position;
                }
            }
        }

        void InitializeStateMachine()
        {
            stateMachine = new AIStateMachine(this);
            
            stateMachine.AddState(new IdleState());
            stateMachine.AddState(new PatrolState());
            stateMachine.AddState(new ChaseState());
            stateMachine.AddState(new AttackState());
            stateMachine.AddState(new SearchState());
            stateMachine.AddState(new FleeState());
            stateMachine.AddState(new DeadState());
        }

        public void TakeDamage(float damage)
        {
            blackboard.health -= damage;
            blackboard.health = Mathf.Max(0f, blackboard.health);
            
            if (blackboard.health <= 0)
            {
                stateMachine.ChangeState(AIStateType.Dead);
            }
        }

        public void AttackPlayer()
        {
            if (blackboard.player != null)
            {
                var playerHealth = blackboard.player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (blackboard.patrolPoints != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < blackboard.patrolPoints.Length; i++)
                {
                    Gizmos.DrawWireSphere(blackboard.patrolPoints[i], 1f);
                    if (i < blackboard.patrolPoints.Length - 1)
                    {
                        Gizmos.DrawLine(blackboard.patrolPoints[i], blackboard.patrolPoints[i + 1]);
                    }
                    else
                    {
                        Gizmos.DrawLine(blackboard.patrolPoints[i], blackboard.patrolPoints[0]);
                    }
                }
            }
        }
    }
}
