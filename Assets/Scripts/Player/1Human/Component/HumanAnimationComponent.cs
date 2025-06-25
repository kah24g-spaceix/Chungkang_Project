using UnityEngine;
public class HumanAnimationComponent : MonoBehaviour
{
    private Animator anim;
    private void Awake() => anim = GetComponent<Animator>();
    public void SetRun(bool v) => anim.SetBool("Run", v);
    public void SetRoll(bool v) => anim.SetBool("Roll", v);
    public void SetAttack(bool v) => anim.SetBool("Attack", v);
}