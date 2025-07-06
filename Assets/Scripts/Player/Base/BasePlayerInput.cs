using UnityEngine;

public class BasePlayerInput : MonoBehaviour, IPlayerInput
{
    [Header("Input Settings")]
    [SerializeField] protected KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] protected KeyCode dodgeKey = KeyCode.LeftShift;
    [SerializeField] protected KeyCode interactKey = KeyCode.E;

    public Vector3 MoveDir { get; private set; }
    public bool IsAttackPressed { get; private set; }
    public bool IsHeavyAttackPressed { get; private set; }
    public bool IsDodgePressed { get; private set; }
    public bool IsInteractPressed { get; private set; }

    protected virtual void Update()
    {
        UpdateMovementInput();
        UpdateActionInput();
    }

    protected virtual void UpdateMovementInput()
    {
        MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MoveDir = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * MoveDir;
    }

    protected virtual void UpdateActionInput()
    {
        IsAttackPressed = Input.GetKeyDown(attackKey);
        IsDodgePressed = Input.GetKeyDown(dodgeKey);
        IsInteractPressed = Input.GetKeyDown(interactKey);
    }

    public void SetHeavyAttackMode(bool enabled)
    {
        IsHeavyAttackPressed = enabled && IsAttackPressed;
    }
}
