using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    private IAttackStrategy _attackStrategy;

    private void Awake()
    {
        _attackStrategy = GetComponent<IAttackStrategy>();
        if (_attackStrategy == null)
            Debug.LogError("공격 전략 없음.");
    }

    public void DoAttack()
    {
        _attackStrategy.Attack();
    }
}
