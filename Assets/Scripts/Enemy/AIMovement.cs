using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI
{
    public class AIMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float walkSpeed = 2f;
        public float runSpeed = 6f;
        public float rotationSpeed = 120f;
        public float stoppingDistance = 1.5f;

        private NavMeshAgent agent;
        private Animator animator;
        private EnemyAIController aiController;

        public bool HasReachedDestination => !agent.pathPending && agent.remainingDistance < stoppingDistance;
        public Vector3 Velocity => agent.velocity;
        public bool IsMoving => agent.velocity.magnitude > 0.1f;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            aiController = GetComponent<EnemyAIController>();

            SetupNavMeshAgent();
        }

        void SetupNavMeshAgent()
        {
            agent.speed = walkSpeed;
            agent.angularSpeed = rotationSpeed;
            agent.acceleration = 8f;
            agent.stoppingDistance = stoppingDistance;
            agent.autoBraking = true;
        }

        public void MoveTo(Vector3 destination, bool isRunning = false)
        {
            if (!agent.enabled) return;

            agent.speed = isRunning ? runSpeed : walkSpeed;
            agent.SetDestination(destination);

            UpdateAnimation();
        }

        public void Stop()
        {
            if (agent.enabled)
            {
                agent.ResetPath();
            }
            UpdateAnimation();
        }

        public void SetMovementEnabled(bool enabled)
        {
            agent.enabled = enabled;
        }

        void Update()
        {
            UpdateAnimation();
            UpdateBlackboard();
        }

        void UpdateAnimation()
        {
            if (animator == null) return;

            float speed = agent.velocity.magnitude;
            float normalizedSpeed = speed / runSpeed;

            animator.SetFloat("Speed", normalizedSpeed);
        }

        void UpdateBlackboard()
        {
            if (aiController?.Blackboard != null)
            {
                aiController.Blackboard.hasReachedDestination = HasReachedDestination;
                aiController.Blackboard.currentDestination = agent.destination;

            }
        }

        public bool FindRandomPoint(Vector3 center, float range, out Vector3 point)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                randomPoint.y = center.y;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    point = hit.position;
                    return true;
                }
            }
            point = center;
            return false;
        }
    }
}