using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UsableItemSO", menuName = 
    "Scriptable Objects/UsableItemSO")]
public class UsableItemSO : ItemSO
{
    public bool PerformAction()
    {
        Debug.Log("Action performed.");
        return true;
    }
}