using UnityEngine;
public class HumanAttack : MonoBehaviour, IAttackStrategy
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    public void Attack()
    {
        Debug.Log("원거리 공격!");
        //Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}