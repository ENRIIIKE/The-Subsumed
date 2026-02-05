using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Parents")]
    public Transform itemSpawnParent;
    public Transform spawnedItemsParent;

    [Header("Items and Spawn Points")]
    public List<Transform> spawnTransform = new List<Transform>();
    public List<ItemsToSpawn> itemsToSpawn = new List<ItemsToSpawn>();
    public List<ItemParameter> parameterToModify = new List<ItemParameter>();

    public int totalAmountToSpawn;

    private void Start()
    {
        if (FindSpawnLocations() == true)
        {
            SpawnItems();
        }
    }

    public bool FindSpawnLocations()
    {
        /* This method identifies all spawn points (children) in the GameObject 
         * that is set in the inspector (parent).
         * 
         * I also added a button to the inspector to find these spawn points.
         * 
         * In case there are not enough spawn points, the game will throw an error 
         * with the required number of spawn points and the current number of spawn points.
         */


        //Debug.LogWarning("Clearing current data and writing new data");

        spawnTransform.Clear();
        totalAmountToSpawn = 0;
        foreach (Transform item in itemSpawnParent)
        {
            spawnTransform.Add(item);
        }

        foreach (ItemsToSpawn item in itemsToSpawn)
        {
            totalAmountToSpawn += item.numberToSpawn;
        }

        if (totalAmountToSpawn > spawnTransform.Count)
        {
            Debug.LogError("Not enough item spawn locations!!!" +
                "\nTotal amount to spawn: " + totalAmountToSpawn +
                "\nTotal spawn points: " + spawnTransform.Count);
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SpawnItems()
    {
        int keyId = 0;

        /* When spawning items, I use a for loop. 
         * The number of items to spawn is set in the inspector.
         */

        for (int i = 0; i < totalAmountToSpawn; i++)
        {
            // Random item from list
            int randomItem = UnityEngine.Random.Range(0, itemsToSpawn.Count);
            // Random location from list
            int randomTransform = UnityEngine.Random.Range(0, spawnTransform.Count);

            // References for better reading
            ItemsToSpawn newItem = itemsToSpawn[randomItem];
            Transform newTransform = spawnTransform[randomTransform];

            GameObject itemGameObject = newItem.item.gameObject;

            Vector3 pos = newTransform.position;
            Vector3 rot = newTransform.eulerAngles;

            /* Creates an object in the scene at the position and rotation 
             * of a randomly selected Transform.
             */


            GameObject newObject = Instantiate(itemGameObject, pos, Quaternion.Euler(rot), 
                spawnedItemsParent);

            /* Modification of item parameters for the object spawned by this script.
             * 
             * The following code works by checking, after the object is spawned,
             * whether it contains any of the parameters we want to modify—
             * these are the parameters added in this script’s inspector.
             * 
             * The item "Key" needs to be modified in a different way—
             * each Key that the game spawns should have a unique ID.
             * So after each key is spawned, the "keyId" integer is incremented.
             */


            List<ItemParameter> itemDefaultState = 
                newObject.GetComponent<Item>().itemSO.parametersList;

            Item itemScript = newObject.GetComponent<Item>();

            foreach (var parameter in parameterToModify)
            {
                if (itemScript.parametersList.Contains(parameter))
                {
                    int index = itemScript.parametersList.IndexOf(parameter);
                    float newValue = 0f;

                    if (itemScript.gameObject.CompareTag("Key"))
                    {
                        newValue = itemScript.parametersList[index].value
                            + keyId;
                        keyId++;
                    }

                    itemScript.parametersList[index] = new ItemParameter
                    {
                        itemParameter = parameter.itemParameter,
                        value = newValue
                    };
                }
            }

            /* Decrease the number of items that need to be spawned
             * If the count of a specific item reaches zero, that item 
             * is removed from the item pool.
             */

            newItem.numberToSpawn--;

            if (newItem.numberToSpawn <= 0)
            {
                itemsToSpawn.Remove(newItem);
            }
            spawnTransform.Remove(newTransform);
        }
    }
}

[Serializable]
public class ItemsToSpawn
{
    public GameObject item;
    public int numberToSpawn;
}
