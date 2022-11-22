using System.Collections;
using System.Collections.Generic;
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

    /* Global prefabs the rest of the UI might need access to */
    public GameObject craftableUIInfoGroupContainer; //Bottom left corner of crafting UI
    public GameObject craftingUIPotionCraftingInputGroup; //Input buttons for bottom right corner of crafting UI





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
