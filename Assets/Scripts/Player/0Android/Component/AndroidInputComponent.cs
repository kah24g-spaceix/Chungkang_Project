using UnityEngine;

public class AndroidInputComponent : BasePlayerInput
{
    private readonly KeyCode androidSpecialKey = KeyCode.Tab;

    private Vector3 _mouseWorldPosition;
    private Vector3 _mouseDirection;

    public Vector3 MouseWorldPosition => _mouseWorldPosition;
    public Vector3 MouseDirection => _mouseDirection;

    public Vector2 MovementBlendValues { get; private set; }
    private Vector2 _currentMovementBlendValues;

    protected override void UpdateActionInput()
    {
        base.UpdateActionInput();

        UpdateMouseDirection();
        UpdateMovementBlendValues();


        HandleAndroidSpecificInput();
    }

    private void UpdateMouseDirection()
    {
        var cam = Camera.main;
        if (cam == null) return;

        Vector3 mouseScreenPos = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mouseScreenPos);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            _mouseWorldPosition = ray.GetPoint(distance);

            Vector3 playerPos = transform.position;
            playerPos.y = 0;
            _mouseWorldPosition.y = 0;

            _mouseDirection = (_mouseWorldPosition - playerPos).normalized;
        }
    }

    private void UpdateMovementBlendValues()
    {
        Vector3 inputDir = MoveDir;

        if (inputDir.magnitude < 0.1f)
        {
            _currentMovementBlendValues = Vector2.zero;
        }
        else
        {
            Vector3 localMovement = transform.InverseTransformDirection(inputDir);
            Vector2 targetBlendValues = new Vector2(localMovement.x, localMovement.z);

            if (targetBlendValues.magnitude > 1f)
                targetBlendValues = targetBlendValues.normalized;

            // Lerp로 값 완만하게 변화
            _currentMovementBlendValues = Vector2.Lerp(_currentMovementBlendValues, targetBlendValues, Time.deltaTime * 10f);
        }

        MovementBlendValues = _currentMovementBlendValues;
    }

    public Vector3 GetDodgeDirection()
    {
        Vector3 dodgeDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) dodgeDir += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) dodgeDir += Vector3.back;
        if (Input.GetKey(KeyCode.A)) dodgeDir += Vector3.left;
        if (Input.GetKey(KeyCode.D)) dodgeDir += Vector3.right;

        return dodgeDir.normalized;
    }

    public Vector3 GetAttackDirection()
    {
        return _mouseDirection;
    }

    public int GetDirectionIndex()
    {
        if (_mouseDirection.magnitude < 0.1f)
            return 0;

        float angle = Mathf.Atan2(_mouseDirection.x, _mouseDirection.z) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        if (angle >= 315 || angle < 45) return 0;
        else if (angle >= 45 && angle < 135) return 1;
        else if (angle >= 135 && angle < 225) return 2;
        else return 3;
    }

    private void HandleAndroidSpecificInput()
    {
        if (Input.GetKeyDown(androidSpecialKey))
        {
            if (Debug.isDebugBuild)
                Debug.Log("Android 전용 기능 활성화");
        }
    }
}
