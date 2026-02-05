using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRigidbody : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField, Range(1f, 3f)] private float _sprintFactor;
    [SerializeField, ReadOnly] private float _currentSpeed;
    [SerializeField, ReadOnly] private bool _canMove = true;

    [Header("Slow Down Settings")]
    [SerializeField] private float _damageSlowDuration;
    [SerializeField, ReadOnly] private bool _isSlowed;
    [SerializeField, ReadOnly] private float _slowTimer;
    private AnimationCurve _damageSlowCurve;

    private Rigidbody _rb;
    private Vector3 _movement;
    private Vector3 _saveScale;

    [Header("Crouch Settings")]
    [SerializeField] private float _crouchLevel = 0.5f;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private Transform _standCheck;
    [SerializeField] private float _standCheckRadius = 0.5f;

    [SerializeField, ReadOnly] private bool _isStanding;
    [SerializeField, ReadOnly] private bool _canStand;

    [Header("Mouse Settings")]
    // Use sensitivity values of 1 - 8 when not using Time.deltaTime
    // Use sensitivity values of 100 - 300 when using Time.deltaTime
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _verticalClamp = 85f;
    [SerializeField, ReadOnly] private bool _canRotate = true;
    private float _xRotation = 0f;

    [Header("Debug Settings")]
    [SerializeField] private TextMeshProUGUI _textSpeed;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (_cameraTransform == null)
        {
            _cameraTransform = Camera.main.transform;
        }

        _currentSpeed = _walkSpeed;
        _isStanding = true;
        _saveScale = transform.localScale;
        
        UpdateDebug();
    }
    void Update()
    {
        if (!_canRotate || !_canMove) return;

        // Get input from WASD keys (A-D Horizontal ; W-S Vertical)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        #region Player direction
        /* Get camera forward/right projected on XZ plane
         * We are doing this to tell the RIGIDBODY where is our FRONT (for z) and RIGHT (for x)
         * When normalized, a vector keeps the same direction but its length is 1.0.
         */
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate movement direction relative to camera
        _movement = (right * moveX + forward * moveZ).normalized;

        #endregion

        #region Crouch

        _canStand = Physics.CheckSphere(_standCheck.position, _standCheckRadius);
        if (Input.GetKeyDown(KeyCode.C))
        {
            //_currentSpeed = _walkSpeed;

            _isStanding = false;
            _currentSpeed = _crouchSpeed;
            UpdateDebug();

            transform.localScale = new Vector3(_saveScale.x, _crouchLevel, _saveScale.z);
        }
        else if (Input.GetKeyUp(KeyCode.C) && _canStand)
        {
            _currentSpeed = _walkSpeed;
            _isStanding = true;
            UpdateDebug();

            transform.localScale = _saveScale;
        }
        // Check if can stand
        if (_canStand && !_isStanding && !Input.GetKey(KeyCode.C))
        {
            _currentSpeed = _walkSpeed;
            _isStanding = true;
            UpdateDebug();

            transform.localScale = _saveScale;
        }
        #endregion

        #region Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift) && _isStanding)
        {
            _currentSpeed = _walkSpeed * _sprintFactor;
            UpdateDebug();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _currentSpeed = _walkSpeed;
            UpdateDebug();
        }
        #endregion

        #region Mouse Look = Update Version =
        // MOUSE LOOK ==== Uupdate Version ====
        /*
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // Rotate camera up/down
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -verticalClamp, verticalClamp);

        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        //_rb.MoveRotation(Quaternion.Euler(0f, mouseX, 0f));
        transform.Rotate(0f, mouseX, 0f);
        */
        #endregion

        #region Slow effect on damage
        // Handle damage slow effect
        if (_isSlowed)
        {
            _slowTimer += Time.deltaTime;
            _currentSpeed = _damageSlowCurve.Evaluate(_slowTimer);

            if (_slowTimer >= _damageSlowDuration)
            {
                _isSlowed = false;
                _slowTimer = 0f;
            }
            UpdateDebug();
        }
        #endregion
    }

    void FixedUpdate()
    {
        if (!_canRotate || !_canMove) return;

        // Rotation update
        Vector3 rotation = new Vector3(transform.eulerAngles.x, 
            _cameraTransform.eulerAngles.y, transform.eulerAngles.z);

        // WASD Movement
        _rb.Move(_rb.position + _movement * _currentSpeed * Time.fixedDeltaTime, 
            Quaternion.Euler(rotation));

        #region Mouse Look = FixedUpdate Version =
        // MOUSE LOOK ==== FixedUpdate Version ====
        /* It looks like there is a difference when using this withTime.deltaTime 
         * (or time.fixedDeltaTime). On display that is set on 144 hertz you need
         * bigger sensitivity than on 60 hertz.
         */
        float mouseX = Input.GetAxisRaw("Mouse X") * _mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * _mouseSensitivity * Time.fixedDeltaTime;

        // Rotate camera up/down
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -_verticalClamp, _verticalClamp);

        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        //_rb.MoveRotation(Quaternion.Euler(0f, mouseX, 0f));
        transform.Rotate(0f, mouseX, 0f);

        #endregion
    }
    public void ToggleMovement(bool toogle)
    {
        _canRotate = toogle;
        _canMove = toogle;
    }
    public void SlowDown()
    {
        _damageSlowCurve = AnimationCurve.EaseInOut(0f, 1f, _damageSlowDuration, _currentSpeed);
        _isSlowed = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_standCheck.position, _standCheckRadius);
    }
    private void UpdateDebug()
    {
        if (_textSpeed != null)
            _textSpeed.text = _currentSpeed.ToString(); // Update the speed text in the UI
    }
}