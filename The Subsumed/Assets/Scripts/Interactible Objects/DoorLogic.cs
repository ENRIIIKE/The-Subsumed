using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(BoxCollider),typeof(HingeJoint))]
public class DoorLogic : MonoBehaviour, IInteractable
{
    // Door General
    [Header("Door General")]
    public bool isLocked;

    // Private variables
    private HingeJoint _hingeJoint;
    private JointMotor _jointMotor;
    private Rigidbody _rb;
    private Transform _playerCamera;

    private bool _isInteracting;
    private float _interactionDirection = 1f;

    public void DefaultSettings()
    {
        /* These settings will be applied only when the button in inspector
         * is pressed 
         */
        gameObject.layer = 8;   // Layer with index "8" is for doors only.

        // Rigidbody
        _rb = GetComponent<Rigidbody>();
        _rb.mass = 8f;
        _rb.linearDamping = 0f;
        _rb.angularDamping = 10f;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Hinge Joint - General
        _hingeJoint = GetComponent<HingeJoint>();
        _hingeJoint.anchor = new Vector3(0f, 0f, 0f);
        _hingeJoint.axis = new Vector3(0f, 1f, 0f);
        _hingeJoint.useMotor = true;

        // Hinge Joint - Limits
        JointLimits limitsValues = _hingeJoint.limits;
        limitsValues.min = -90f;
        limitsValues.max = 90f;
        _hingeJoint.limits = limitsValues;

        // Hinge Joint - Spring
        JointSpring springValues = _hingeJoint.spring;
        springValues.spring = 20f;
        springValues.damper = 20f;
        _hingeJoint.spring = springValues;

        // Door Logic
        _playerCamera = Camera.main.transform;
    }

    private void Start()
    {
        _hingeJoint = GetComponent<HingeJoint>();
        _rb = GetComponent<Rigidbody>();

        _playerCamera = Camera.main.transform;
        if (isLocked)
        {
            LockDoor();
        }
    }
    void FixedUpdate()
    {
        if (_isInteracting)
        {
            float mouseX = Input.GetAxisRaw("Mouse X");

            _jointMotor = _hingeJoint.motor;
            _jointMotor.force = 250f; // Strength of a push
            _jointMotor.targetVelocity = mouseX * 500 * _interactionDirection;
            _hingeJoint.motor = _jointMotor;
        }
    }
    public void Interact()
    {
        // Check if its locked


        Debug.LogWarning("Interacting with the door");

        // Direction correction
        Vector3 cameraCorrectDirection = _playerCamera.transform.position - transform.position;
        Vector3 hingeCorrectDirection = transform.up;

        // Cross product to determine if I want to push the door or pull
        // Cross product returns a vector that is perpendicular to both hingeCorrectDirection and cameraCorrectDirection
        Vector3 crossProduct = Vector3.Cross(hingeCorrectDirection, cameraCorrectDirection);
        //Debug.LogWarning("Cross product: " + crossProduct);


        // Dot product retursn a scalar (number) representing how aligned two vectors are
        _interactionDirection = Mathf.Sign(Vector3.Dot(crossProduct, transform.forward));
        //Debug.LogWarning("Intearaction direction: " + _interactionDirection);

        _isInteracting = true;
    }
    public void DeInteract()
    {
        if (isLocked) return;

        _isInteracting = false;

        _jointMotor = _hingeJoint.motor;
        _jointMotor.force = 0;
        _jointMotor.targetVelocity = 0;
        _hingeJoint.motor = _jointMotor;
    }
    public void UnlockDoor()
    {
        Debug.Log("Unlocking door", gameObject);
        isLocked = false;

        JointLimits jointLimits = _hingeJoint.limits;

        jointLimits.max = 90f;
        jointLimits.min = -90f;

        _hingeJoint.limits = jointLimits;
    }
    public void LockDoor()
    {
        JointLimits jointLimits = _hingeJoint.limits;

        jointLimits.max = 0f;
        jointLimits.min = 0f;

        _hingeJoint.limits = jointLimits;
    }
}