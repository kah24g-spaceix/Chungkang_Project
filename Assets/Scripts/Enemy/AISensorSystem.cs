using UnityEngine;
using System.Collections.Generic;

namespace EnemyAI
{
    public class AISensorSystem : MonoBehaviour
    {
        [Header("Vision Settings")]
        public float sightRange = 15f;
        public float sightAngle = 60f;
        public LayerMask obstacleLayer = -1;
        public LayerMask playerLayer = -1;

        [Header("Hearing Settings")]
        public float hearingRange = 20f;

        private EnemyAIController aiController;
        private Transform playerTransform;

        void Awake()
        {
            aiController = GetComponent<EnemyAIController>();
        }

        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        public void UpdateSensors()
        {
            if (playerTransform == null) return;

            bool canSeePlayer = CanSeePlayer();
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            var blackboard = aiController.Blackboard;
            blackboard.player = playerTransform;
            blackboard.canSeePlayer = canSeePlayer;
            blackboard.distanceToPlayer = distanceToPlayer;

            if (canSeePlayer)
            {
                blackboard.lastKnownPlayerPosition = playerTransform.position;
                blackboard.lastSeenPlayerTime = Time.time;
                blackboard.alertLevel = Mathf.Min(1f, blackboard.alertLevel + Time.deltaTime * 2f);
            }
            else
            {
                blackboard.alertLevel = Mathf.Max(0f, blackboard.alertLevel - Time.deltaTime * 0.5f);
            }
            blackboard.isAlerted = blackboard.alertLevel > 0.3f;
        }

        bool CanSeePlayer()
        {
            if (playerTransform == null) return false;

            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > sightRange) return false;

            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer > sightAngle / 2) return false;

            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, directionToPlayer, out hit, distanceToPlayer, obstacleLayer | playerLayer))
            {
                return hit.collider.CompareTag("Player");
            }

            return false;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);

            Vector3 leftBoundary = Quaternion.AngleAxis(-sightAngle / 2, Vector3.up) * transform.forward * sightRange;
            Vector3 rightBoundary = Quaternion.AngleAxis(sightAngle / 2, Vector3.up) * transform.forward * sightRange;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, hearingRange);
        }
    }
}