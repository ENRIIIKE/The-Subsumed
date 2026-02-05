using System;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    public PatrolPointData patrolPointData;
}

[Serializable]
public class PatrolPointData
{
    public string Name;
    public Transform PointTransform;
    public PatrolPointType PointType;
    [Tooltip("This transform is used for the enemy to look at when it reaches this point. It can be null if not needed.")]
    public Transform ActionDirection; // Optional, used for ActionPoints
    public int IndexOfAnimation = -1;   // -1 means no animation assigned
}
public enum PatrolPointType
{
    Waypoint,
    ActionPoint
}