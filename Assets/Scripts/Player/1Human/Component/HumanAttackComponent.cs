using UnityEngine;
public class HumanAttackComponent : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public void Attack() => Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
}