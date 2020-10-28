using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Text;

public class ItemSlot : MonoBehaviour
{
    // Event callbacks
    public UnityEvent<Item> onItemUse;

    // flag to tell ItemSlot it needs to update itself after being changed
    private bool b_needsUpdate = true;
    public bool recipeSlot = false;

    // Declared with auto-property
    public Item ItemInSlot { get; private set; }
    public int ItemCount { get; private set; }

    private Crafting craftingSystem;

    // scene references
    [SerializeField]
    private TMPro.TextMeshProUGUI itemCountText;

    [SerializeField]
    public Image itemIcon;

    private void Start()
    {
        craftingSystem = GameObject.FindWithTag("Player").GetComponent<Crafting>();
    }

    private void Update()
    {
        if(b_needsUpdate)
        {
            UpdateSlot();
        }
    }

    /// <summary>
    /// Returns true if there is an item in the slot
    /// </summary>
    /// <returns></returns>
    public bool HasItem()
    {
        return ItemInSlot != null;
    }

    /// <summary>
    /// Removes everything in the item slot
    /// </summary>
    /// <returns></returns>
    public void ClearSlot()
    {
        ItemInSlot = null;
        ItemCount = 0;
        b_needsUpdate = true;
    }

    /// <summary>
    /// Attempts to remove a number of items. Returns number removed
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public int TryRemoveItems(int count)
    {
        if(count > ItemCount)
        {
            int numRemoved = ItemCount;
            ItemCount -= numRemoved;
            b_needsUpdate = true;
            return numRemoved;
        } else
        {
            ItemCount -= count;
            b_needsUpdate = true;
            return count;
        }
    }

    /// <summary>
    /// Sets what is contained in this slot
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void SetContents(Item item, int count)
    {
        ItemInSlot = item;
        ItemCount = count;
        b_needsUpdate = true;
    }

    /// <summary>
    /// Activate the item currently held in the slot
    /// </summary>
    public void UseItem()
    {
        if(ItemInSlot != null)
        {
            if(ItemCount >= 1)
            {
                ItemInSlot.Use();
                onItemUse.Invoke(ItemInSlot);
                ItemCount--;
                b_needsUpdate = true;
            }
        }
    }

    public void SelectItem()
    {
        if (ItemInSlot != null)
        {
            if (ItemCount >= 1 && !craftingSystem.isItemSelected)
            {
                itemIcon.color = new Color32(64, 212, 255, 255);
                craftingSystem.SelectItem(this, ItemInSlot, ItemCount);
            }
            else if (ItemCount >= 1 && craftingSystem.isItemSelected && !recipeSlot)
            {
                if (ItemInSlot.Name == craftingSystem.selectedItem.Name)
                {
                    craftingSystem.StackItem(this);
                }
                else
                {
                    craftingSystem.SwitchItem(this);
                }
            }
        }
        else if (ItemInSlot == null && craftingSystem.isItemSelected)
        {
            craftingSystem.PlaceItem(this);
        }
    }
    /// <summary>
    /// Update visuals of slot to match items contained
    /// </summary>
    private void UpdateSlot()
    {
        itemCountText.text = ItemCount.ToString();

        if (ItemCount == 0)
        {
            ItemInSlot = null;
        }

        if(ItemInSlot != null)
        {
            //itemCountText.text = ItemCount.ToString();
            itemIcon.sprite = ItemInSlot.Icon;
            itemCountText.fontSize = 24;
            //itemIcon.gameObject.SetActive(true);
        }

        else
        {
            itemIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            itemCountText.fontSize = 0;
        }

        b_needsUpdate = false;
    }
}
