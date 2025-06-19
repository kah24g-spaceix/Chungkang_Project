using UnityEngine;

public class WalkStrategy : IMovementStrategy
{
    public void Move(Rigidbody rigidbody, Vector3 direction, float speed, float deltaTime)
    {
        Vector3 moveDir = direction.normalized * speed * deltaTime;
        rigidbody.MovePosition(rigidbody.position + moveDir);
    }
}
