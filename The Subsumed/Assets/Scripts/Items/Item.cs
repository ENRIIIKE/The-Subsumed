using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public ItemSO itemSO;
    public List<ItemParameter> parametersList;

    public void Interact()
    {
        PlayerInventory.instance.AddItemToInventory(itemSO, parametersList);
        Destroy(gameObject);
    }
    public void DeInteract()
    {
        // Doesn't work without this bullshit
    }
}