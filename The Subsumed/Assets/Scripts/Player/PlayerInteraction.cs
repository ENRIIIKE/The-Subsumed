using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float _interactDistance;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Image _crosshair; // Reference to crosshair 
    [SerializeField, ReadOnly] private bool _canInteract = true;

    private IInteractable _lockedObject;
    private IInteractable _lookingAtObject;

    void Update()
    {
        if (!_canInteract) return;

        RaycastHit hit;
        if (Physics.Raycast(_cameraTransform.position,
            _cameraTransform.TransformDirection(Vector3.forward), out hit, _interactDistance))
        {
            IInteractable interactableObject = hit.collider.GetComponent<IInteractable>();

            if (interactableObject != null)
            {
                _crosshair.enabled = true;
                _lookingAtObject = interactableObject;

                //Debug.Log("Confirmed hit on: " + hit.collider.gameObject.name,
                    //hit.collider.gameObject);
            }
            else
            {
                _crosshair.enabled = false;
                _lookingAtObject = null;
            }
        }
        else
        {
            _crosshair.enabled = false;
            _lookingAtObject = null;
        }

        /* Draw a line from the player to visually recognize in the editor 
         * when the player is looking at an interactable object.
         * 
         * Yellow = No interactable object in range.
         * Green = Interactable object in range.
         */


        if (_lookingAtObject == null)
        {
            Debug.DrawRay(_cameraTransform.position,
                _cameraTransform.TransformDirection(Vector3.forward) * _interactDistance,
                Color.yellow);
        }
        else
        {
            Debug.DrawRay(_cameraTransform.position,
                _cameraTransform.TransformDirection(Vector3.forward) * hit.distance,
                  Color.green);
        }

        // Player input (Interaction key)
        if (Input.GetKeyDown(KeyCode.E))
        {
            _lockedObject = _lookingAtObject;
            if (_lockedObject == null) return;
            _lockedObject.Interact();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            if (_lockedObject == null) return;
            _lockedObject.DeInteract();
            _lockedObject = null;
        }
    }
    public void ToggleInteract(bool toggle)
    {
        _canInteract = toggle;
    }
}
