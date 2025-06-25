using UnityEngine;
public class HumanInputComponent : MonoBehaviour
{
    public Vector3 MoveDir { get; private set; }
    private void Update()
    {
        MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MoveDir = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * MoveDir;
    }
}