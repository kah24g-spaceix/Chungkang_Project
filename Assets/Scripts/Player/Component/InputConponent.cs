using UnityEngine;

public class InputComponent : MonoBehaviour
{
    public Vector3 MoveDirection { get; private set; }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        MoveDirection = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0)
                        * new Vector3(h, 0, v).normalized;
    }
}
