using UnityEngine;

public class BubbleGizmo : MonoBehaviour
{
    [SerializeField] private Color bubbleColor = Color.green;
    [SerializeField] private float radius = 0.05f;
    [SerializeField] private bool showGizmo = true;
    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Gizmos.color = bubbleColor;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
