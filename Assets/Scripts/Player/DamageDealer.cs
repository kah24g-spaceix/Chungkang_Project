using UnityEngine;
using System.Collections.Generic;

public class DamageDealer : MonoBehaviour
{
    [HideInInspector] public int damageAmount;
    
    [Header("Damage Settings")]
    [SerializeField] private LayerMask enemyLayers = -1;
    [SerializeField] private bool debugMode = true;
    [SerializeField] private float damageDelay = 0.1f; // 연속 데미지 방지
    
    // 이미 데미지를 준 적들을 추적 (한 번의 공격당 한 번만 데미지)
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    private float lastDamageTime;
    
    void OnEnable()
    {
        // 무기가 활성화될 때마다 데미지 받은 적 목록 초기화
        damagedEnemies.Clear();
        lastDamageTime = Time.time;
    }
    
    void OnTriggerEnter(Collider other)
    {
        // 데미지 딜레이 체크
        if (Time.time - lastDamageTime < damageDelay) return;
        
        // 이미 데미지를 준 적인지 확인
        if (damagedEnemies.Contains(other.gameObject)) return;
        
        // Enemy 레이어 체크
        if (!IsInLayerMask(other.gameObject.layer, enemyLayers)) return;
        
        // Enemy 태그 확인
        if (!other.CompareTag("Enemy")) return;
        
        // EnemyAI 컴포넌트 찾기
        EnemyAI enemyAI = other.GetComponent<EnemyAI>();
        if (enemyAI == null)
        {
            // 부모에서 찾기 시도
            enemyAI = other.GetComponentInParent<EnemyAI>();
        }
        
        if (enemyAI != null)
        {
            // 데미지 적용
            enemyAI.TakeDamage(damageAmount);
            
            // 이 적에게 데미지를 줬다고 기록
            damagedEnemies.Add(other.gameObject);
            lastDamageTime = Time.time;
            
            if (debugMode)
            {
                Debug.Log($"Weapon hit {other.name} for {damageAmount} damage!");
            }
            
            // 타격 효과 (선택사항)
            CreateHitEffect(other.transform.position);
        }
        else
        {
            if (debugMode)
            {
                Debug.LogWarning($"Hit object {other.name} with Enemy tag but no EnemyAI component found!");
            }
        }
    }
    
    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }
    
    private void CreateHitEffect(Vector3 position)
    {
        // 간단한 파티클 효과나 사운드 재생
        // 나중에 파티클 시스템으로 교체 가능
        if (debugMode)
        {
            Debug.Log($"Hit effect at position: {position}");
        }
    }
    
    // 공격이 끝날 때 호출 (AndroidAttackComponent에서 호출)
    public void ResetDamageTracking()
    {
        damagedEnemies.Clear();
    }
    
    void OnDisable()
    {
        // 무기가 비활성화될 때 추적 초기화
        damagedEnemies.Clear();
    }
    
    // 디버그용 정보 표시
    void OnDrawGizmos()
    {
        if (debugMode && GetComponent<Collider>() != null)
        {
            Collider col = GetComponent<Collider>();
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            if (col is CapsuleCollider capsule)
            {
                Gizmos.DrawWireCube(capsule.center, new Vector3(capsule.radius * 2, capsule.height, capsule.radius * 2));
            }
            else if (col is BoxCollider box)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }
    }
}
