using UnityEngine;

[RequireComponent(typeof(PlayerStateMachine))]
public class InputComponent : MonoBehaviour
{
    private void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            EventBus.OnRoll?.Invoke();
        }
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v).normalized;
        EventBus.OnMoveInputChanged?.Invoke(dir.magnitude);
    }
}