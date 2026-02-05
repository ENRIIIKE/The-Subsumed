using UnityEngine;

public class ItemSpawnVisualization : MonoBehaviour
{
    [SerializeField] private bool _showGizmos = true;
    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.16f);

        Gizmos.color = Color.white;
        Debug.DrawRay(transform.position, transform.forward);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.16f);

        Gizmos.color = Color.white;
        Debug.DrawRay(transform.position, transform.forward);
    }
}