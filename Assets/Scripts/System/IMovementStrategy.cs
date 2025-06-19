using UnityEngine;

public interface IMovementStrategy
{
    void Move(Rigidbody rigidbody, Vector3 direction, float speed, float deltaTime);
}
