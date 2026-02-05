using UnityEngine;

public abstract class InspectObject : MonoBehaviour, IInteractable
{
    [Header("Inspect Settings")]
    public bool isInspecting = false;
    public bool showLog = false;
    public GameObject inspectCanvas;
    public GameObject original;

    [SerializeField] private Transform _inspectTransform;
    [SerializeField, Range(1, 8)] int _scaleAmount;

    private GameObject _instance;

    public void DeInteract()
    {
        // Doesn't work without this bullshit
    }
    public void Interact()
    {
        if (!isInspecting)
        {
            EnableInspect();
        }
        else if (isInspecting)
        {
            DisableInspect();
        }
    }

    public void EnableInspect()
    {
        // Disable movement
        PlayerSettings.instance.ToggleLinks(false);

        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Zoom in the inspected object and if there are secondary
        // functions for that object, enable them.
        _instance = Instantiate(gameObject,
            _inspectTransform.transform.position,
            Quaternion.identity, _inspectTransform);

        // Apply scale for bette visual
        _instance.transform.localScale *= _scaleAmount;

        // Adjust the scripts in created gameobject
        _instance.GetComponent<InspectObject>().
            InstantiateSettings(gameObject);
        _instance.AddComponent<InspectMovement>();
    }
    public void DisableInspect()
    {
        // Enable movement
        PlayerSettings.instance.ToggleLinks(true);

        // Destroy inspected object.
        if (showLog) Debug.Log("Destroy");
        Destroy(gameObject, 0.02f);

        // Lock in the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public virtual void InstantiateSettings(GameObject originalObject)
    {
        // Initial settings for inspected object
        original = originalObject;
        inspectCanvas.SetActive(true);
        isInspecting = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, transform.localScale * 0.7f);
    }

}