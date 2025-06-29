using UnityEngine;

public class AndroidAnimationComponent : BasePlayerAnimation
{
    [Header("Attack Animation Parameters")]
    [SerializeField] private string comboIndexParameter = "ComboIndex";
    [SerializeField] private string isHeavyAttackParameter = "IsHeavyAttack";
    [SerializeField] private string attackTriggerParameter = "AttackTrigger";
    
    // Combo attack method
    public void SetComboAttack(int comboIndex)
    {
        if (animator != null)
        {
            animator.SetInteger(comboIndexParameter, comboIndex);
            animator.SetBool(isHeavyAttackParameter, false);
            animator.SetBool("IsAttacking", true);
            animator.SetTrigger(attackTriggerParameter);
        }
    }
    
    // Heavy attack method
    public void SetHeavyAttack(int comboIndex)
    {
        if (animator != null)
        {
            animator.SetInteger(comboIndexParameter, comboIndex);
            animator.SetBool(isHeavyAttackParameter, true);
            animator.SetBool("IsAttacking", true);
            animator.SetTrigger(attackTriggerParameter);
        }
    }
    
    // Override base attack method to use combo system
    public override void SetAttack(bool isAttacking)
    {
        if (animator != null)
        {
            animator.SetBool("IsAttacking", isAttacking);
            
            // 공격이 끝나면 콤보 관련 파라미터 리셋
            if (!isAttacking)
            {
                animator.SetInteger(comboIndexParameter, 0);
                animator.SetBool(isHeavyAttackParameter, false);
            }
        }
    }
}
