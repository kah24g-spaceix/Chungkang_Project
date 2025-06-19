using System.Collections;
using UnityEngine;

public class MoveSpeedItem : MonoBehaviour
{
    public float buffAmount = 2.0f;
    public float buffDuration = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var movement = other.GetComponent<MovementComponent>();
            if (movement != null)
            {
                StartCoroutine(ApplyBuff(movement));
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator ApplyBuff(MovementComponent movement)
    {
        movement.moveSpeed += buffAmount;
        EventBus.OnMoveSpeedChanged?.Invoke(movement.moveSpeed);

        yield return new WaitForSeconds(buffDuration);

        movement.moveSpeed -= buffAmount;
        EventBus.OnMoveSpeedChanged?.Invoke(movement.moveSpeed);
    }
}