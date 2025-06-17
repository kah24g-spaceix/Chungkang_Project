using UnityEngine;

public interface IMovementStrategy
{
    void Move(CharacterController cc, float deltaTime);
}

public interface IState
{
    void Enter();
    void Excute(float dt);
    void Exit();
}