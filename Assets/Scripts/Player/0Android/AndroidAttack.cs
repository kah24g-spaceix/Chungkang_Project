using UnityEngine;

public class AndroidAttack : MonoBehaviour, IAttackStrategy
{
    public void Attack()
    {
        Debug.Log("근접 공격!");
    }
}
