using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "AI/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("Health")]
    public float maxHealth = 100f;
    [Range(0f, 1f)]
    public float fleeHealthPercentage = 0.3f;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 120f;

    [Header("Normal Detection (기본 상태)")]
    public float sightRange = 10f;
    [Range(30f, 180f)]
    public float sightAngle = 60f;

    [Header("Alerted Detection (플레이어 인식 후)")]
    public float alertedSightRange = 15f;
    [Range(60f, 360f)]
    public float alertedSightAngle = 120f;

    [Header("Sight Transition")]
    public float sightTransitionSpeed = 2f;
    public float alertedStateDuration = 5f;

    [Header("Detection Layers")]
    public LayerMask playerLayer = 1;
    public LayerMask obstacleLayer = 1;

    [Header("Combat")]
    public float attackRange = 2f;
    public int attackDamage = 20;
    public float attackCooldown = 2f;
    public float attackDuration = 1f;
    [Tooltip("애니메이션 재생 후 실제 데미지 적용까지의 지연시간")]
    public float attackDamageDelay = 0.5f;

    [Header("Patrol")]
    public float patrolWaitTime = 3f;
    public float patrolRadius = 5f;
    [Tooltip("패트롤 지점 도착 판정 거리")]
    public float patrolArrivalDistance = 1f;

    [Header("Flee")]
    [Tooltip("도망칠 때 목표까지의 거리")]
    public float fleeDistance = 10f;
    [Tooltip("NavMesh 샘플링 반경")]
    public float fleeSampleRadius = 10f;
    [Tooltip("도망 중 멈출 거리")]
    public float fleeStopDistance = 1f;

    [Header("Investigation")]
    public float investigationTime = 3f;
    public float investigationRotationSpeed = 45f;
    [Tooltip("수색 지점 도착 판정 거리")]
    public float investigationArrivalDistance = 1f;
}
