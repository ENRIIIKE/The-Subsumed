using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 300f;
    [SerializeField] private float _verticalClamp = 85f;

    private float _xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * _mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * (_mouseSensitivity * 0.5f) 
            * Time.fixedDeltaTime;

        // Rotate camera up/down
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -_verticalClamp, _verticalClamp);

        transform.localRotation = Quaternion.Euler(_xRotation, mouseX, 0f);
    }   
}