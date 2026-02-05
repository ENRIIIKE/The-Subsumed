using UnityEngine;

public class SightCone : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _playerInCone;

    public bool PlayerInCone
    {
        get { return _playerInCone; }
        private set { _playerInCone = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInCone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInCone = false;
        }
    }
}
