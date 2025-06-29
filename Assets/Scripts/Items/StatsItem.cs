using System.Collections;
using UnityEngine;

public class StatsItem : MonoBehaviour
{
    public int hp = 0;
    public int ep = 0;
    public int attackPower = 0;
    public float speed = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                var playerStats = other.GetComponent<IPlayerStats>();
                if (playerStats != null)
                {
                    playerStats.AddAttackPower(attackPower);
                    playerStats.AddSpeed(speed);
                }
                Destroy(gameObject);
            }
        }
    }
}