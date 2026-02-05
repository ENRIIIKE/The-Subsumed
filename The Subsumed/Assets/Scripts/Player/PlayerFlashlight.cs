using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField] private Light _light;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _light.enabled = !_light.enabled;
        }
    }
}
