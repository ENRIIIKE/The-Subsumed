using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    // Reference to the gameobject that is visually showing selected item.
    [SerializeField] private GameObject _holdingItem;

    // This prefab will be created when player drops object.
    [SerializeField] private GameObject emptyPrefab;

    // Storage for inventory items.
    [SerializeField] private InventoryItem[] _inventoryItems = new InventoryItem[5];

    // The currently selected slot in the inventory.
    [SerializeField, ReadOnly] private int _selectedSlot = 0;

    // Hand animator for the player.
    [SerializeField] private Animator _playerHandAnimator;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    [Header("DOTween")]
    [SerializeField] private float _distanceToMove;
    [SerializeField] private float _duration;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private int _numberOfItemsInInventory = 0;

    [Header("HUD")]
    [SerializeField] private Image[] _inventoryImages = new Image[3];
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private GameObject _inventoryUI;

    #region Singleton
    // This singleton will be used to access this script throughout the
    // entire scene.

    public static PlayerInventory instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }
    #endregion

    private void Start()
    {
        _meshFilter = _holdingItem.GetComponent<MeshFilter>();
        _meshRenderer = _holdingItem.GetComponent<MeshRenderer>();

        _endPosition = _holdingItem.transform.localPosition;
        _startPosition = new Vector3(_endPosition.x, _endPosition.y - _distanceToMove,
            _endPosition.z);

        _inventoryUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            DropItem(_selectedSlot);
        }
    }
    
    private void ToggleInventory()
    {
        // Pause the game, but it needs a little bit of a touch to make it feel like you are opening the inventory.
        PlayerSettings.instance.PauseGame();

        // Enable UI for the inventory.
        _inventoryUI.SetActive(!_inventoryUI.activeSelf);
        bool toggle = _inventoryUI.activeSelf;

        /* - first, UI needs to be enabled, so the player can see it. I am thinking of the canvas being attached to the
         * player hand, so it will be moving with the hand, this could be a good idea, giving it a  little bit of life.
         * - then the animation will be played, which will make the player hand move close to the camera.
         */
        _playerHandAnimator.SetBool("Inventory", toggle);
        
        if (toggle == false)
        {
            // Play animation depending on the selected item.
            int itemID = _inventoryItems[_selectedSlot].itemSO != null 
                ? _inventoryItems[_selectedSlot].itemSO.ID 
                : -1;

            _playerHandAnimator.SetInteger("SelectedItem", itemID);

            // WARNING: itemID with -1 will not play because there is no animation for it.
        }
    }

    /// <summary>
    /// Attempts to add an item to the inventory.
    /// </summary>
    /// <remarks>If the inventory is full, the method logs a message indicating that no more items can be
    /// added.</remarks>
    /// <param name="item">The item to add to the inventory. Cannot be <see langword="null"/>.</param>
    /// <param name="parameterList">A list of parameters associated with the item. Can be <see langword="null"/> if no parameters are required.</param>
    /// <returns><see langword="true"/> if the item was successfully added to the inventory; otherwise, <see langword="false"/>
    /// if the inventory is full.</returns>
    public bool AddItemToInventory(ItemSO item, List<ItemParameter> parameterList)
    {
        for (int i = 0; i < _inventoryItems.Length; i++)
        {
            if (_inventoryItems[i].itemSO == null)
            {
                _inventoryItems[i].itemSO = item;
                _inventoryItems[i].parameterList = parameterList;

                if (_selectedSlot == i)
                {
                    UpdateSelectedItem(i);
                }
                _numberOfItemsInInventory++;
                UpdateInventoryUI();
                return true;
            }
        }

        Debug.Log("Inventory full");
        return false;
    }

    // Remove item from inventory completely
    public void RemoveSelectedItem()
    {
        _inventoryItems[_selectedSlot].RemoveThisItem();
        _numberOfItemsInInventory--;
        DefaultVisuals();
    }

    /// <summary>
    /// Drops an item from the inventory at the specified index and spawns it in the game world.
    /// </summary>
    /// <remarks>The item is instantiated at a position slightly below the player's current position, with
    /// adjustments to ensure proper placement. The item's visual appearance and parameters are set based on the
    /// inventory data. A box collider is added to the dropped item for interaction in the game world. After the item is
    /// dropped, the inventory is updated, and the player's HUD and visuals are reset.</remarks>
    /// <param name="indexOfItem">The zero-based index of the item in the inventory to be dropped. Must correspond to a valid item in the
    /// inventory.</param>
    private void DropItem(int indexOfItem)
    {
        /* - It would be better to use a raycast from the player straight downward. 
         * - On hit, the position will be stored and then moved slightly upward— 
         * the amount of upward movement will be half of the item's Y scale.
         */

        Vector3 dropPos = new Vector3(transform.position.x,
            transform.position.y - 1.47f, transform.position.z);

        GameObject droppedItem = Instantiate(emptyPrefab, dropPos,
            Quaternion.identity);

        Item itemScript = droppedItem.GetComponent<Item>();
        itemScript.itemSO = _inventoryItems[indexOfItem].itemSO;
        itemScript.parametersList = _inventoryItems[indexOfItem].parameterList;

        droppedItem.GetComponent<MeshFilter>().mesh = 
            _inventoryItems[indexOfItem].itemSO.meshFilter.sharedMesh;
        droppedItem.GetComponent<MeshRenderer>().material =
            _inventoryItems[indexOfItem].itemSO.meshRenderer.sharedMaterial;

        BoxCollider collider = droppedItem.AddComponent<BoxCollider>();

        collider.size = collider.size * 1.4f;

        UpdateInventoryUI();
        RemoveSelectedItem();
        DefaultVisuals();
    }
    
    private void UpdateSelectedItem(int index)
    {
        int previousSlot = _selectedSlot;
        _selectedSlot = index;

        if (_inventoryItems[index].itemSO != null)
        {
            _meshFilter.mesh = 
                _inventoryItems[index].itemSO.meshFilter.sharedMesh;
            _meshRenderer.material = 
                _inventoryItems[index].itemSO.meshRenderer.sharedMaterial;

            _holdingItem.transform.localPosition = _startPosition;
            _holdingItem.transform.DOLocalMove(_endPosition, _duration).SetEase(Ease.Linear);
        }
        
        else
        {
            _meshFilter.mesh = null;
            _meshRenderer.material = null;
        }
        
    }

    
    private void UpdateInventoryUI()
    {
        /* 
        int previousIndex = 0;
        int afterIndex = 0;

        for (int i = 5; i > _inventoryItems.Length; i++)
        {
            if (_inventoryItems[i].itemSO == null)
            {
                continue;
            }
            else
            {
                previousIndex = i;
                _inventoryImages[0].sprite = _inventoryItems[i].itemSO.itemSprite ?? _defaultSprite;
                break;
            }
        }

        for (int i = 0; i < _inventoryItems.Length; i--)
        {
            if (_inventoryItems[i].itemSO == null)
            {
                continue;
            }
            else
            {
                afterIndex = i;
                _inventoryImages[2].sprite = _inventoryItems[i].itemSO.itemSprite ?? _defaultSprite;
                break;
            }
        }

        _inventoryImages[1].sprite = _inventoryItems[_selectedSlot].itemSO.itemSprite ?? _defaultSprite;
        */


        int previousIndex = (_selectedSlot - 1 + _inventoryItems.Length) % _inventoryItems.Length;
        while (_inventoryItems[previousIndex].itemSO == null)
        {
            previousIndex--;
            if (previousIndex < 0)
            {
                previousIndex = _inventoryItems.Length - 1;
            }
        }

        int afterIndex = (_selectedSlot + 1) % _inventoryItems.Length;
        while (_inventoryItems[afterIndex].itemSO == null)
        {
            afterIndex++;
            if (afterIndex >= _inventoryItems.Length)
            {
                afterIndex = 0;
            }
        }

        // Debug.Log(previousIndex + " " + _selectedSlot + " " + afterIndex);

        // Above slot (previous item)
        _inventoryImages[0].sprite = _inventoryItems[previousIndex].itemSO.itemSprite ?? null;

        // Center slot (current item)
        _inventoryImages[1].sprite = _inventoryItems[_selectedSlot].itemSO.itemSprite ?? null;

        // Below slot (next item)
        _inventoryImages[2].sprite = _inventoryItems[afterIndex].itemSO.itemSprite ?? null;

    }
    public void NextItem()
    {
        _selectedSlot++;
        if (_selectedSlot >= _inventoryItems.Length)
        {
            _selectedSlot = 0;
        }

        while (_inventoryItems[_selectedSlot].itemSO == null)
        {
            _selectedSlot++;
            if (_selectedSlot >= _inventoryItems.Length)
            {
                _selectedSlot = 0;
            }
        }

        UpdateInventoryUI();
    }
    public void PreviousItem()
    {
        _selectedSlot--;
        if (_selectedSlot < 0)
        {
            _selectedSlot = _inventoryItems.Length - 1;
        }

        while (_inventoryItems[_selectedSlot].itemSO == null)
        {
            _selectedSlot--;
            if (_selectedSlot < 0)
            {
                _selectedSlot = _inventoryItems.Length - 1;
            }
        }

        UpdateInventoryUI();
    }

    public InventoryItem ReturnItem()
    {
        return _inventoryItems[_selectedSlot];
    }
    private void DefaultVisuals()
    {
        _meshFilter.mesh = null;
        _meshRenderer.material = null;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_holdingItem.transform.position, 0.2f);
    }
}

[Serializable]
public class InventoryItem
{
    public ItemSO itemSO;
    public List<ItemParameter> parameterList;

    public void RemoveThisItem()
    {
        itemSO = null;
        parameterList = null;
    }
} 
