using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftingUIPotionsManager : MonoBehaviour {
    private static CraftingUIPotionsManager instance;
    public static CraftingUIPotionsManager Instance {  get { return instance; } }

    //Bottom half UI containers for tab #1
    [SerializeField]
    private GameObject CraftingUIInfoContainer_BottomLeftCorner;
    [SerializeField]
    private GameObject CraftingUIActionContainer_BottomRightCorner;

    public int itemID { get; set; } = -1;
    public int amountToCraft { get; private set; } = 1;
    private bool itemCraftable { get; set; } = false; //private variable referring to whether a potion is craftable

    /* Global prefabs the rest of the UI might need access to */
    public GameObject craftableUIInfoGroupContainer; //Ready-made prefab for bottom left corner of crafting UI
    public GameObject craftingUIPotionCraftingInputGroup; //Ready-made prefab for Input buttons for bottom right corner of crafting UI



    private void Awake() {
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
    }



    //Called every time a potion scroll view element is clicked
    public void LoadCraftableUI() {
        clearAllCraftableUIInfo();
        ShowCraftableItemInfo();
        ShowCraftableItemAction();

    }

    /***********************************************************Potion Info**************************************************************/

    private void clearAllCraftableUIInfo() {
        //GameManager.clearAllChildrenOfObj(CraftingUIInfoContainer_BottomLeftCorner);
        GameManager.clearAllChildrenOfObj(CraftingUIActionContainer_BottomRightCorner.transform.GetChild(0));
    }

    private void ShowCraftableItemInfo() {
        //var CraftingUIInfoContainer = GameObject.FindGameObjectWithTag("CraftingUIInfoContainer");
        Transform CraftableItemInfoGroup;
        if (CraftingUIInfoContainer_BottomLeftCorner.transform.childCount == 0) {
            //Then we need to instantiate the prefab
            CraftableItemInfoGroup = Instantiate(this.craftableUIInfoGroupContainer, CraftingUIInfoContainer_BottomLeftCorner.transform).transform;
        }
        else {
            CraftableItemInfoGroup = CraftingUIInfoContainer_BottomLeftCorner.transform.GetChild(0);
        }
        var craftableUIInfoGroupContainerController = CraftableItemInfoGroup.GetComponent<CraftableUIInfoGroupContainerController>();
        CraftableItemInfoGroup.gameObject.SetActive(true);

        var item = InGameItemsDatabaseManager.Instance.getItemByID(this.itemID);

        craftableUIInfoGroupContainerController.SetIcon(item.TwoDimensionalPrefab);
        craftableUIInfoGroupContainerController.SetItemName(item.itemName);
        craftableUIInfoGroupContainerController.SetItemDescription(item.ItemDescription);
        craftableUIInfoGroupContainerController.SetItemStatsText("    Effect Radius: " + item.EffectRadius + "\n    Effect Timeout: " + item.EffectTimeout + " sec");
    }


    /***********************************************************Potion Action**************************************************************/

    public void UpdateCraftingRecipeTable(int amountToCraft) {
        this.amountToCraft = amountToCraft;
        GameManager.clearAllChildrenOfObj(this.craftableItemRecipeTable);
        var itemToCraft = InGameItemsDatabaseManager.Instance.getItemByID(itemID);
        var arrOfRecipeItems = itemToCraft.Recipe;
        this.itemCraftable = true;
        bool recipeRequirementsMet = CraftingUIController.fillOutRecipeTable(this.craftableItemRecipeTable, arrOfRecipeItems, amountToCraft);
        if (!recipeRequirementsMet) { this.itemCraftable = false; }
        //Show XP required
        TextMeshProUGUI xpText;
        if (craftableItemXPRequiredTextbox != null) {
            Destroy(craftableItemXPRequiredTextbox);
        }
            craftableItemXPRequiredTextbox = new GameObject("Required XP");
            xpText = craftableItemXPRequiredTextbox.AddComponent<TextMeshProUGUI>();
            //Set font size to 10
            xpText.fontSize = 10f;
            xpText.font = GameManager.Instance.CRAFTINGUI_costTextFont;
            xpText.verticalAlignment = VerticalAlignmentOptions.Middle;

            var xpTextRectTransform = craftableItemXPRequiredTextbox.GetComponent<RectTransform>();
            xpTextRectTransform.SetParent(CraftingUIActionContainer_BottomRightCorner.transform.GetChild(0), false);
            
            xpTextRectTransform.sizeDelta = new Vector2(xpTextRectTransform.sizeDelta.x, 30);



        var xpCost = InGameItemsDatabaseManager.Instance.getItemByID(itemID).XPCost * amountToCraft; //Don't forget to factor in the amount they are trying to make

        xpText.text = "XP: <color=\"" + ((XPSystem.Instance.XP < xpCost) ? "red" : "green") + "\">" + XPSystem.Instance.XP + "</color>/" + xpCost;

        if (xpCost > XPSystem.Instance.XP) {
            this.itemCraftable = false;
        }

        if (itemToCraft.Stackable) {
            if (InventoryManager.Instance.getCountOfRemainingOpenSlots() < 1) {
                //We have no room in our inventory, do not allow the craft
                this.itemCraftable = false;
            }
        }
        else {
            if (InventoryManager.Instance.getCountOfRemainingOpenSlots() < amountToCraft) {
                //We have no room in our inventory, do not allow the craft
                this.itemCraftable = false;
            }
        }
        //Last number check to make sure that 
        if (!(amountToCraft > 0)) {
            this.itemCraftable= false;
        }
        if (inputGroupController != null) {
            inputGroupController.SetAmountToCraft(amountToCraft);
            if (this.itemCraftable) {
                inputGroupController.EnableCraftingButton();
            }
            else {
                inputGroupController.DisableCraftingButton();
            }
        }
    }
    private GameObject craftableItemRecipeTable;
    private GameObject craftableItemXPRequiredTextbox;
    CraftingUIPotionCraftingInputGroupController inputGroupController;
    private void ShowCraftableItemAction(int amountToCraft = 1) {

        craftableItemRecipeTable = Table.createNewTable(CraftingUIActionContainer_BottomRightCorner.transform.GetChild(0), 220, 100);
        craftableItemRecipeTable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -10); //Bring it 10px down for padding

        var inputGroup = Instantiate(this.craftingUIPotionCraftingInputGroup, CraftingUIActionContainer_BottomRightCorner.transform.GetChild(0));
        inputGroupController = inputGroup.GetComponent<CraftingUIPotionCraftingInputGroupController>();

        UpdateCraftingRecipeTable(amountToCraft);        
        
    }
    public void CraftPotion(int amountToCraft) {
        if (this.itemCraftable) {
            GetComponent<CraftingManager>().Craft(this.itemID, amountToCraft); //Pass it on to the attached Crafting Manager script
            UpdateCraftingRecipeTable(amountToCraft);
        }
    }



}