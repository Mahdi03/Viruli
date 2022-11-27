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

    public int itemID { get; set; }
    private bool itemCraftable { get; set; } = false; //private variable referring to whether a potion is craftable

    [SerializeField]
    private Color tableBorderColor = Color.white;
    [SerializeField]
    private Color tablePaddingColor = Color.HSVToRGB(213 / 360f, 17 / 100f, 21 / 100f);

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
    private void clearAllCraftableUIInfo() {
        //GameManager.clearAllChildrenOfObj(CraftingUIInfoContainer_BottomLeftCorner);
        GameManager.clearAllChildrenOfObj(CraftingUIActionContainer_BottomRightCorner);
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
        //TODO: Next time we might need to instantiate the prefab and not just find it in the hierarchy depending on how the walls system works
        var craftableUIInfoGroupContainerController = CraftableItemInfoGroup.GetComponent<CraftableUIInfoGroupContainerController>();
        CraftableItemInfoGroup.gameObject.SetActive(true);

        var item = InGameItemsDatabaseManager.Instance.getItemByID(this.itemID);

        craftableUIInfoGroupContainerController.SetIcon(item.TwoDimensionalPrefab);
        craftableUIInfoGroupContainerController.SetItemName(item.itemName);
        craftableUIInfoGroupContainerController.SetItemDescription(item.ItemDescription);
        craftableUIInfoGroupContainerController.SetItemStatsText("- Effect Radius: " + item.EffectRadius + "ft\n- Effect Timeout: " + item.EffectTimeout + "sec");
    }
    
    public void UpdateCraftingRecipeTable(int amountToCraft) {
        GameManager.clearAllChildrenOfObj(this.craftableItemRecipeTable);
        var arrOfRecipeItems = InGameItemsDatabaseManager.Instance.getItemByID(itemID).Recipe;
        this.itemCraftable = true;
        foreach (var item in arrOfRecipeItems) {
            var id = item.Item1;
            var requiredItem = InGameItemsDatabaseManager.Instance.getItemByID(id);
            var countRequired = item.Item2 * amountToCraft;
            var countAvailable = InventoryManager.Instance.getItemCountByID(id);
            //Make a row
            var row = Table.createTableRow(this.craftableItemRecipeTable.transform, 30f);
            //In the first cell instantiate the prefab
            var iconCell = Table.createTableCell(row.transform, cellWidth: 30f, tableBorderColor, borderWidth: 1f, tablePaddingColor);
            var requiredItemIcon = Instantiate(requiredItem.TwoDimensionalPrefab, iconCell.transform);
            //Add hover tooltip
            IItem.attachItemInstance(requiredItemIcon, id); //Give the 2D Icon an ID so that the HoverTooltip can read it
            IItem.enableScript<OnHoverTooltip>(requiredItemIcon);

            //In the second cell put in the amount required
            var requiredAmountCell = Table.createTableCell(row.transform, cellWidth: 190f, tableBorderColor, borderWidth: 1f, tablePaddingColor);
            var textbox = new GameObject("Required Amount");

            var text = textbox.AddComponent<TextMeshProUGUI>();
            text.text = "<color=\"" + ((countAvailable < countRequired) ? "red" : "green") + "\">" + countAvailable + "</color>/" + countRequired;
            //Set font size to 16
            text.fontSize = 16f;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
            if (countAvailable < countRequired) {
                this.itemCraftable = false;
            }

            var rectTransform = textbox.GetComponent<RectTransform>();
            rectTransform.SetParent(requiredAmountCell.transform, false);
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);

            //Give 5px padding on the left
            rectTransform.offsetMin = new Vector2(5, 0); //(Left, Bottom)
            rectTransform.offsetMax = new Vector2(-0, -0); //(-Right, -Top)
        }
        //Show XP required
        TextMeshProUGUI xpText;
        if (craftableItemXPRequiredTextbox != null) {
            //Delete it and reinstantiate a new one
            Destroy(craftableItemXPRequiredTextbox);
        }

        craftableItemXPRequiredTextbox = new GameObject("Required XP");

        xpText = craftableItemXPRequiredTextbox.AddComponent<TextMeshProUGUI>();

        //Set font size to 16
        xpText.fontSize = 16f;
        xpText.verticalAlignment = VerticalAlignmentOptions.Middle;

        var xpTextRectTransform = craftableItemXPRequiredTextbox.GetComponent<RectTransform>();
        xpTextRectTransform.SetParent(CraftingUIActionContainer_BottomRightCorner.transform, false);

        var xpCost = InGameItemsDatabaseManager.Instance.getItemByID(itemID).XPCost * amountToCraft; //Don't forget to factor in the amount they are trying to make

        xpText.text = "XP: <color=\"" + ((XPSystem.Instance.XP < xpCost) ? "red" : "green") + "\">" + XPSystem.Instance.XP + "</color>/" + xpCost;

        if (xpCost > XPSystem.Instance.XP) {
            this.itemCraftable = false;
        }

        inputGroupController.SetAmountToCraft(amountToCraft);
        if (this.itemCraftable) {
            inputGroupController.EnableCraftingButton();
        }
        else {
            inputGroupController.DisableCraftingButton();
        }
    }
    private GameObject craftableItemRecipeTable;
    private GameObject craftableItemXPRequiredTextbox;
    CraftingUIPotionCraftingInputGroupController inputGroupController;
    private void ShowCraftableItemAction(int amountToCraft = 1) {
        //TODO: Show "Reach Level _ to unlock this spell first"

        craftableItemRecipeTable = Table.createNewTable(CraftingUIActionContainer_BottomRightCorner.transform, 220, 100);
        craftableItemRecipeTable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -10); //Bring it 10px down for padding

        var inputGroup = Instantiate(this.craftingUIPotionCraftingInputGroup, CraftingUIActionContainer_BottomRightCorner.transform);
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
