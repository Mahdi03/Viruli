using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CraftingUIController : MonoBehaviour {
    public GameObject scrollView;
    public GameObject scrollViewElementPrefab;

    private void Awake() {
        populateScrollViewWithSpells(); //TODO: fix initialiation code later
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
        }
    }

    public void populateScrollViewWithWalls() { }

    /// <summary>
    /// Cleans out all the old choices already present in the scrollview
    /// </summary>
    public void emptyScrollView() {
        GameManager.clearAllChildrenOfObj(scrollView);
    }
}
