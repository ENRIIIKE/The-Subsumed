using UnityEngine;

public class ConeExample : MonoBehaviour
{
    [Header("Vision Cone Settings")]
    public float visionRange = 10f;
    [Range(1f, 180f)]
    public float visionAngle = 60f;
    public Color coneColor = new Color(1f, 1f, 0f, 0.3f);

    private Mesh _coneMesh;

    private void OnValidate()
    {
        // Generate the cone mesh with appropriate radius
        float radius = visionRange * Mathf.Tan(Mathf.Deg2Rad * visionAngle * 0.5f);
        _coneMesh = ConeMeshGenerator.CreateCone(visionRange, radius, 24);
    }

    private void OnDrawGizmos()
    {
        if (_coneMesh == null)
        {
            OnValidate();
        }

        if (_coneMesh != null)
        {
            Gizmos.color = coneColor;

            // Draw the cone as a wireframe
            //Gizmos.DrawWireMesh(_coneMesh, transform.position, transform.rotation);
            Gizmos.DrawWireMesh(_coneMesh, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }
    }
}
