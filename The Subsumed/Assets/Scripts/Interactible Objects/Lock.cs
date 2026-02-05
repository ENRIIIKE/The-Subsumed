using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Lock : Interactable
{
    public void DeleteItem()
    {
        PlayerInventory.instance.RemoveSelectedItem();
    }
    public void AnimationEnd()
    {
        GetComponentInParent<DoorLogic>().UnlockDoor();
        Destroy(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.6f);
    }
}