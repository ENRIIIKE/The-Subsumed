using UnityEngine;

public class HandsFollow : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float smoothTime = 1f;
    [SerializeField] private float rotateTime = 1f;

    private Vector3 velocity = Vector3.zero;
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetToFollow.position, ref velocity, smoothTime * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetToFollow.rotation, rotateTime * Time.deltaTime);
    }
}
