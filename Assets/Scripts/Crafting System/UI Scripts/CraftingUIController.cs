using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CraftingUIController : MonoBehaviour {

    private static CraftingUIController instance;
    public static CraftingUIController Instance { get { return instance; } }


    [SerializeField]
    private GameObject scrollView;
    [SerializeField]
    private GameObject scrollViewElementPrefab;

    [SerializeField]
    private GameObject CraftingUIInfoContainer_BottomLeftCorner;
    [SerializeField]
    private GameObject CraftingUIActionContainer_BottomRightCorner;

    public int itemID { get; set; }
    private bool craftable { get; set; } = false;

    /* Global prefabs the rest of the UI might need access to */
    public GameObject craftableUIInfoGroupContainer; //Bottom left corner of crafting UI
    public GameObject craftingUIPotionCraftingInputGroup; //Input buttons for bottom right corner of crafting UI

    public void LoadCraftableUI() {
        clearAllInfo();
        ShowCraftableItemInfo();
        ShowCraftableItemAction();

    }
    protected void clearAllInfo() {
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
    [SerializeField]
    private Color tableBorderColor = Color.white;
    [SerializeField]
    private Color tablePaddingColor = Color.HSVToRGB(213 / 360f, 17 / 100f, 21 / 100f);
    public void UpdateRecipeTable(int amountToCraft) {
        GameManager.clearAllChildrenOfObj(this.table);
        var arrOfRecipeItems = InGameItemsDatabaseManager.Instance.getItemByID(itemID).Recipe;
        this.craftable = true;
        foreach (var item in arrOfRecipeItems) {
            var id = item.Item1;
            var requiredItem = InGameItemsDatabaseManager.Instance.getItemByID(id);
            var countRequired = item.Item2 * amountToCraft;
            var countAvailable = InventoryManager.Instance.getItemCountByID(id);
            //Make a row
            var row = Table.createTableRow(this.table.transform, 30f);
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
                this.craftable = false;
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


        inputGroupController.SetAmountToCraft(amountToCraft);
        if (this.craftable) {
            inputGroupController.EnableCraftingButton();
        }
        else {
            inputGroupController.DisableCraftingButton();
        }
    }
    private GameObject table;
    CraftingUIPotionCraftingInputGroupController inputGroupController;
    private void ShowCraftableItemAction(int amountToCraft = 1) {

        table = Table.createNewTable(CraftingUIActionContainer_BottomRightCorner.transform, 220, 100);
        table.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -10); //Bring it 10px down for padding
        
        var inputGroup = Instantiate(this.craftingUIPotionCraftingInputGroup, CraftingUIActionContainer_BottomRightCorner.transform);
        inputGroupController = inputGroup.GetComponent<CraftingUIPotionCraftingInputGroupController>();
        UpdateRecipeTable(amountToCraft);        
    }
    public void CraftPotion(int amountToCraft) {
        if (this.craftable) {
            GetComponent<CraftingManager>().Craft(this.itemID, amountToCraft); //Pass it on to the attached Crafting Manager script
            UpdateRecipeTable(amountToCraft);
        }
    }

    private void Awake() {
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;

            populateScrollViewWithSpells(); //TODO: fix initialiation code later
        }
        
    }

    /// <summary>
    /// This function will populate the scroll view with all the craftable items (potions namely)
    /// </summary>
    public void populateScrollViewWithSpells() {
        //Clean out all of the old children and replace with new one
        emptyScrollView();
        foreach (var keyValuePair in InGameItemsDatabaseManager.Instance.craftableItems) {
            var newScrollViewElement = Instantiate(scrollViewElementPrefab, scrollView.transform);

            ScrollViewElementController newScrollViewElementController = newScrollViewElement.GetComponent<ScrollViewElementController>();
            newScrollViewElementController.setIcon(keyValuePair.Value.TwoDimensionalPrefab);
            newScrollViewElementController.setText(keyValuePair.Value.itemName);
            newScrollViewElementController.setItemID(keyValuePair.Value.ID);
        }
        /*Testing out new table library
        var aTransform = scrollView.transform.parent.parent.parent.parent.GetChild(1).GetChild(0);
        var table = Table.createNewTable(aTransform, tableWidth: 220, tableHeight: 100);
        for (int i = 0; i < 2; i++) {
            //Create rows
            var row = Table.createTableRow(table.transform, 30f);
            for (int j = 0; j < 5; j++) {
                var tableCell = Table.createTableCell(row.transform, 50f, borderColor: Color.blue, borderWidth: 1f, paddingColor: Color.white);
            }
        }
        */
        
    }

    public void populateScrollViewWithWalls() { }

    /// <summary>
    /// Cleans out all the old choices already present in the scrollview
    /// </summary>
    public void emptyScrollView() {
        GameManager.clearAllChildrenOfObj(scrollView);
    }
}
