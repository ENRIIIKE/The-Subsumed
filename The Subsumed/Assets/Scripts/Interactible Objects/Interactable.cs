using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [Header("General settings")]
    public bool _showLog;
    [SerializeField] private bool _needSpecificItem;
    [SerializeField] private InventoryItem _specificItem;
    [SerializeField] private ItemParameter _specificParameter;

    [Space(20f)]
    public UnityEvent interactEvents;

    public void DeInteract()
    {
        // Doesn't work without this bullshit
    }
    public void Interact()
    {
        /* Check if the player has the correct item (and/or correct parameter) in 
         * the hand, if so, execute "interactEvents".
         */
        if (_needSpecificItem)
        {
            if (!CorrectItem())
            {
                return;
            }
        }

        interactEvents.Invoke();
    }
    
    private bool CorrectItem()
    {
        InventoryItem inventoryItem = PlayerInventory.instance.ReturnItem();
        if (inventoryItem.itemSO == null) return false;

        // Check if items IDs match
        if (_specificItem.itemSO.ID != inventoryItem.itemSO.ID)
        {
            if (_showLog) Debug.Log("Wrong Item!");
            return false;
        }

        if (_showLog) Debug.Log("IDs do match!");

        // Check if item in hand has parameters.
        if (_specificParameter.itemParameter == null)
        {
            if (_showLog) Debug.LogWarning("No parameter required," +
                "proceed with ID match only.");
            return true;
        }

        // Check if any parameter matches
        /* There might be a problem when requiring mulitple item parameters, if one 
         * parameter matches, then it won't compare the other ones.
         */
        foreach (ItemParameter paramter in inventoryItem.parameterList)
        {
            if (_specificParameter.value == paramter.value)
            {
                if (_showLog) Debug.Log("Paramaters match!");
                return true;
            }
        }
        if (_showLog) Debug.LogWarning("Parameters don't match!");
        return false;
    }
}
