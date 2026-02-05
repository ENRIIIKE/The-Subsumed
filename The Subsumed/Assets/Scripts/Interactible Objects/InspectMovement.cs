using UnityEngine;

public class InspectMovement : MonoBehaviour
{
    private Vector3 _lastMousePosition;
    private float _rotationSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition - _lastMousePosition;
            float rotationX = mousePos.y * _rotationSpeed * Time.deltaTime;
            float rotationY = mousePos.x * _rotationSpeed * Time.deltaTime;

            Quaternion rotation = Quaternion.Euler(rotationX, -rotationY, 0);
            transform.rotation = rotation * transform.rotation;

            _lastMousePosition = Input.mousePosition;
        }
    }
}
