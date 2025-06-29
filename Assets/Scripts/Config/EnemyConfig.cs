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
    [Tooltip("플레이어를 인식했을 때의 확장된 시야 거리")]
    public float alertedSightRange = 15f;
    [Range(60f, 360f)]
    [Tooltip("플레이어를 인식했을 때의 확장된 시야 각도")]
    public float alertedSightAngle = 120f;
    
    [Header("Sight Transition")]
    [Tooltip("시야 전환 속도")]
    public float sightTransitionSpeed = 2f;
    [Tooltip("경계 상태 지속 시간 (플레이어를 놓친 후)")]
    public float alertedStateDuration = 5f;
    
    [Header("Detection Layers")]
    public LayerMask playerLayer = 1;
    public LayerMask obstacleLayer = 1;
    
    [Header("Combat")]
    public float attackRange = 2f;
    public int attackDamage = 20;
    public float attackCooldown = 2f;
    public float attackDuration = 1f;
    
    [Header("Patrol")]
    public float patrolWaitTime = 3f;
    public float patrolRadius = 5f;
    
    [Header("Investigation")]
    public float investigationTime = 3f;
    public float investigationRotationSpeed = 45f;
}
