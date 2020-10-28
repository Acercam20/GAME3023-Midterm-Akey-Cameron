using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // For finding all gameObjects with name
using UnityEngine.Assertions;
//using System.Diagnostics;

public class Crafting : MonoBehaviour, ISaveHandler
{
    [Tooltip("Reference to the master item table")]
    [SerializeField]
    private ItemTable masterItemTable;

    [Tooltip("The object which will hold Item Slots as its direct children")]
    [SerializeField]
    private GameObject inventoryPanel;

    [Tooltip("List size determines how many slots there will be. Contents will replaced by copies of the first element")]
    [SerializeField]
    private List<ItemSlot> itemSlots;

    [Tooltip("Items to add on Start for testing purposes")]
    [SerializeField]
    private List<Item> itemRecipes;

    private string saveKey = "";

    public bool isItemSelected;
    public Item selectedItem;
    private int selectedItemQuantity;
    private ItemSlot selectedItemSlot;

    public int amountOfRecipes;
    public List<string> recipeMasterList;

    // Start is called before the first frame update
    void Start()
    {
        InitItemSlots();
        InitRecipes();
        itemSlots[7].recipeSlot = true;
    }

    private void InitRecipes()
    {
        recipeMasterList.Add("/"); //Blue Potion
        recipeMasterList.Add("/"); //Blue Shell
        recipeMasterList.Add("/"); //Bread
        recipeMasterList.Add("/"); //Carrot
        recipeMasterList.Add("/"); //Clover
        recipeMasterList.Add("/"); //Grapes
        recipeMasterList.Add("/"); //Green Potion
        recipeMasterList.Add("ClothClothClothCloth/Cloth///"); //Cloth Hat
        recipeMasterList.Add("Iron///IronIron/IronIron"); //Iron Ball
        recipeMasterList.Add("/"); //Lemon
        recipeMasterList.Add("////Carrot//Water/"); //Orange Potion
        recipeMasterList.Add("/"); //Pink Potion
        recipeMasterList.Add("/"); //Red Pepper
        recipeMasterList.Add("////Red Pepper//Water/"); //Red Potion
        recipeMasterList.Add("/"); //Water
        recipeMasterList.Add("////Lemon//Water/"); //Yellow Potion
        recipeMasterList.Add("/"); //Cloth
        recipeMasterList.Add("///Cloth/ClothCloth/Cloth"); //Cloth Boots
        recipeMasterList.Add("Cloth/ClothClothClothClothClothClothCloth"); //Cloth Armour
        recipeMasterList.Add("/"); //Iron
        recipeMasterList.Add("IronIron/IronIron///Iron"); //Iron Key
    }

    public void CheckForRecipe()
    {
        bool ingrediantsExist = false;
        string tempString = "";
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i != 3 && i != 7 && i != 11)
            {
                if(itemSlots[i].ItemInSlot != null && itemSlots[i].ItemCount > 0)
                {
                    tempString += itemSlots[i].ItemInSlot.Name;
                }
                else
                {
                    tempString += "/";
                }
            }
        }
        Debug.Log(tempString);
        for (int i = 0; i < recipeMasterList.Count; i++)
        {
            if (tempString == recipeMasterList[i])
            {
                CraftRecipe(i);
                ingrediantsExist = true;
            }
        }
        if (!ingrediantsExist)
        {
            itemSlots[7].ClearSlot();
        }
    }

    private void CraftRecipe(int recipeIndex)
    {
        itemSlots[7].SetContents(masterItemTable.GetItem(recipeIndex), 1);
        itemSlots[7].itemIcon.color = new Color32(255, 255, 255, 255);
    }

    public void SelectItem(ItemSlot slot, Item item, int quant)
    {

        if(!slot.recipeSlot)
        {
            isItemSelected = true;
            selectedItem = item;
            selectedItemQuantity = quant;
            selectedItemSlot = slot;
        }
        else
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (i != 7)
                {
                    itemSlots[i].TryRemoveItems(1);
                    isItemSelected = true;
                    selectedItem = item;
                    selectedItemQuantity = quant;
                    selectedItemSlot = slot;
                }
            }
        }
        CheckForRecipe();
    }

    public void PlaceItem(ItemSlot slotToPlace)
    {
        if (!slotToPlace.recipeSlot)
        {
            slotToPlace.SetContents(selectedItem, 1);
            slotToPlace.itemIcon.color = new Color32(255, 255, 255, 255);
            selectedItemSlot.TryRemoveItems(1);
            selectedItemSlot.itemIcon.color = new Color32(255, 255, 255, 255);
            isItemSelected = false;
            selectedItem = null;
            selectedItemQuantity = 0;
            //selectedItemSlot.ClearSlot();
        }
        CheckForRecipe();
    }

    public void StackItem(ItemSlot slotToStackOn)
    {
        slotToStackOn.SetContents(slotToStackOn.ItemInSlot, selectedItemQuantity + slotToStackOn.ItemCount);
        slotToStackOn.itemIcon.color = new Color32(255, 255, 255, 255);
        selectedItemSlot.TryRemoveItems(1);
        selectedItemSlot.itemIcon.color = new Color32(255, 255, 255, 255);
        isItemSelected = false;
        selectedItem = null;
        selectedItemQuantity = 0;
        //selectedItemSlot.ClearSlot();
        CheckForRecipe();
    }

    public void SwitchItem(ItemSlot slotToPlace)
    {
        selectedItemSlot.SetContents(slotToPlace.ItemInSlot, slotToPlace.ItemCount);
        selectedItemSlot.itemIcon.color = new Color32(255, 255, 255, 255);

        slotToPlace.SetContents(selectedItem, selectedItemQuantity);
        slotToPlace.itemIcon.color = new Color32(255, 255, 255, 255);

        isItemSelected = false;
        selectedItem = null;
        selectedItemQuantity = 0;

        CheckForRecipe();
    }

    private void InitItemSlots()
    {
        Assert.IsTrue(itemSlots.Count > 0, "itemSlots was empty");
        Assert.IsNotNull(itemSlots[0], "Inventory is missing a prefab for itemSlots. Add it as the first element of its itemSlot list");

        // init item slots
        for (int i = 1; i < itemSlots.Count; i++)
        {
            GameObject newObject = Instantiate(itemSlots[0].gameObject, inventoryPanel.transform);
            ItemSlot newSlot = newObject.GetComponent<ItemSlot>();
            itemSlots[i] = newSlot;
        }

        foreach (ItemSlot item in itemSlots)
        {
            item.onItemUse.AddListener(OnItemUsed);
        }
    }

    void OnItemUsed(Item itemUsed)
    {
        // Debug.Log("Inventory: item used of category " + itemUsed.category);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSave()
    {
        //Make empty string
        //For each item slot
        //Get its current item
        //If there is an item, write its id, and its count to the end of the string
        //If there is not an item, write -1 and 0 

        //File format:
        //ID,Count,ID,Count,ID,Count

        string saveStr = "";

        foreach (ItemSlot itemSlot in itemSlots)
        {
            int id = -1;
            int count = 0;

            if (itemSlot.HasItem())
            {
                id = itemSlot.ItemInSlot.ItemID;
                count = itemSlot.ItemCount;
            }

            saveStr += id.ToString() + ',' + count.ToString() + ',';
        }

        PlayerPrefs.SetString(saveKey, saveStr);
    }

    public void OnLoad()
    {
        //Get save string
        //Split save string
        //For each itemSlot, grab a pair of entried (ID, count) and parse them to int
        //If ID is -1, replace itemSlot's item with null
        //Otherwise, replace itemSlot with the corresponding item from the itemTable, and set its count to the parsed count

        string loadedData = PlayerPrefs.GetString(saveKey, "");

        Debug.Log(loadedData);

        char[] delimiters = new char[] { ',' };
        string[] splitData = loadedData.Split(delimiters);

        for (int i = 0; i < itemSlots.Count; i++)
        {
            int dataIdx = i * 2;

            int id = int.Parse(splitData[dataIdx]);
            int count = int.Parse(splitData[dataIdx + 1]);

            if (id < 0)
            {
                itemSlots[i].ClearSlot();
            }
            else
            {
                itemSlots[i].SetContents(masterItemTable.GetItem(id), count);
            }
        }
    }
}
