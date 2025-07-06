public enum BlackboardKey
{
    // Basic Components
    Player,
    Agent,
    Config,
    Transform,
    Animator,
    AndroidStats,
    
    // Player Detection
    IsPlayerVisible,
    DistanceToPlayer,
    LastKnownPlayerPosition,
    
    // Health & Combat
    CurrentHealth,
    HealthPercentage,
    LastAttackTime,
    IsAttacking,
    AttackTimer,
    DamageApplied,
    
    // Sight System
    CurrentSightRange,
    CurrentSightAngle,
    SightState,
    
    // Patrol System
    SpawnPosition,
    PatrolPoints,
    CurrentPatrolTarget,
    CurrentPatrolIndex,
    IsPatrolWaiting,
    PatrolWaitTimer,
    
    // Flee System
    HasFleeTarget,
    FleeTarget,
    
    // Investigation System
    HasReachedPosition,
    InvestigateTimer
}
