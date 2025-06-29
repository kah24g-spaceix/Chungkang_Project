using UnityEngine;
public class HumanAttackComponent : MonoBehaviour, IPlayerAttack
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    public void Attack()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}
