// AndroidAnimationComponent.cs
using UnityEngine;

public class AndroidAnimationComponent : BasePlayerAnimation
{

    [Header("Attack Parameters")]
    private string comboIndexParameter = "ComboIndex";
    private string isHeavyAttackParameter = "IsHeavyAttack";
    private string attackTriggerParameter = "Attack";
    private string isAttackingParameter = "IsAttacking";

    /// <summary>
    /// 캐릭터 이동 상태에 따른 Blend Tree 파라미터 업데이트
    /// (BasePlayerAnimation.UpdateMovementBlend 를 사용)
    /// </summary>
    // 애니메이션 업데이트 메서드는 BasePlayerAnimation 에 이미 구현되어 있습니다.

    /// <summary>
    /// 방향성 있는 콤보 공격
    /// </summary>
    public void SetDirectionalComboAttack(int comboIndex, int direction)
    {
        animator.SetInteger(comboIndexParameter, comboIndex);
        animator.SetInteger("AttackDirection", direction);
        animator.SetBool(isHeavyAttackParameter, false);
        animator.SetBool(isAttackingParameter, true);
        animator.SetTrigger(attackTriggerParameter);
    }

    /// <summary>
    /// 방향성 있는 강공격
    /// </summary>
    public void SetDirectionalHeavyAttack(int comboIndex, int direction)
    {
        animator.SetInteger(comboIndexParameter, comboIndex);
        animator.SetInteger("AttackDirection", direction);
        animator.SetBool(isHeavyAttackParameter, true);
        animator.SetBool(isAttackingParameter, true);
        animator.SetTrigger(attackTriggerParameter);
    }

    /// <summary>
    /// 간단한 공격 상태 토글
    /// </summary>
    public override void SetAttack(bool isAttacking)
    {
        animator.SetBool(isAttackingParameter, isAttacking);
        if (!isAttacking)
        {
            animator.SetInteger(comboIndexParameter, 0);
            animator.SetBool(isHeavyAttackParameter, false);
        }
    }

    /// <summary>
    /// 방향 인덱스를 벡터로 변환
    /// </summary>
    public Vector2 GetDirectionVector(int direction)
    {
        switch (direction)
        {
            case 0: return new Vector2(0, 1);    // 앞
            case 1: return new Vector2(1, 0);    // 오른쪽
            case 2: return new Vector2(0, -1);   // 뒤
            case 3: return new Vector2(-1, 0);   // 왼쪽
            default: return Vector2.zero;
        }
    }
}
