using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUITabsManager : MonoBehaviour {
    private static CraftingUITabsManager instance;
    public static CraftingUITabsManager Instance { get => instance; }

    [SerializeField]
    private GameObject[] tabButtons;
    [SerializeField]
    private GameObject[] tabContents;



    [SerializeField]
    private GameObject scrollViewContent1;

    [SerializeField]
    private GameObject scrollViewContent2;

    [SerializeField]
    private GameObject scrollViewElementPrefab; //Flexible prefab works for both scroll view elements when the onclick handler is extended


    private void Awake() {
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            //Initialization code goes here

            //Set buttons to be associated with ID's
            for (int i = 0; i < tabButtons.Length; i++) {
                var tabButton = tabButtons[i];
                var tabContent = tabContents[i];
                IItem.attachItemInstance(tabButton, i);
                IItem.attachItemInstance(tabContent, i);
            }


            populateScrollViewWithSpells(scrollViewContent1);
            populateScrollViewWithDoors(scrollViewContent2);
            //For starters select tab #1

            SelectTab(0);


        }
    }

    /********************************************************Tabs***********************************************************************/
    //Global methods to hide tabs
    public void UnSelectAllTabButtons() {
        foreach (var tabButton in tabButtons) {
            tabButton.GetComponent<TabButtonController>().UnselectButton();
        }
    }
    public void HideAllTabContents() {
        foreach (var tabContent in tabContents) {
            tabContent.SetActive(false);
        }
    }


    public void SelectTab(int tabID) {
        SelectTabButton(tabID);
        ShowTabContent(tabID);
        if (tabID != 1) {
            //If we are selecting another tab than tab #2, then make sure to unglow any of the doors
            MainDoorManager.Instance.UnglowAllDoors();
        }
    }

    private void SelectTabButton(int i) {
        UnSelectAllTabButtons();
        tabButtons[i].GetComponent<TabButtonController>().SelectButton();
    }
    private void ShowTabContent(int i) {
        HideAllTabContents();
        tabContents[i].SetActive(true);
    }


    /// <summary>
    /// This function will populate the given scroll view with all the craftable items (potions namely)
    /// </summary>
    /// <param name="scrollView"></param>
    public void populateScrollViewWithSpells(GameObject scrollView) {
        //Clean out all of the old children and replace with new one
        emptyScrollView(scrollView);
        foreach (var keyValuePair in InGameItemsDatabaseManager.Instance.craftableItems) {
            var newScrollViewElement = Instantiate(scrollViewElementPrefab, scrollView.transform);

            ScrollViewElementController newScrollViewElementController = newScrollViewElement.GetComponent<ScrollViewElementController>();
            PotionScrollViewElement newPotionScrollViewElementController = IItem.enableScript<PotionScrollViewElement>(newScrollViewElement); //Add the potion click callback
            IItem.disableScript<ScrollViewElementController>(newScrollViewElement);
            newPotionScrollViewElementController.setIcon(keyValuePair.Value.TwoDimensionalPrefab);
            newPotionScrollViewElementController.setText(keyValuePair.Value.itemName);
            newPotionScrollViewElementController.setItemID(keyValuePair.Value.ID);
        }
    }
    /// <summary>
    /// This function will populate the given scroll view with all the doors we have available in the database manager
    /// </summary>
    /// <param name="scrollView"></param>
    public void populateScrollViewWithDoors(GameObject scrollView) {
        emptyScrollView(scrollView);
        foreach (var mainDoor in InGameItemsDatabaseManager.Instance.mainDoors) {
            var newScrollViewElement = Instantiate(scrollViewElementPrefab, scrollView.transform);

            ScrollViewElementController newScrollViewElementController = newScrollViewElement.GetComponent<ScrollViewElementController>();
            DoorScrollViewElement newDoorScrollViewElementController = IItem.enableScript<DoorScrollViewElement>(newScrollViewElement); //Add the door click callback
            IItem.disableScript<ScrollViewElementController>(newScrollViewElement);
            //TODO: Set 2D door icon
            //newScrollViewElementController.setIcon(keyValuePair.Value.TwoDimensionalPrefab);
            newDoorScrollViewElementController.setText(mainDoor.doorName);
            newDoorScrollViewElementController.setItemID(mainDoor.ID);
        }
    }
    /************************************************Helper Methods********************************************************/

    /// <summary>
    /// Cleans out all the old choices already present in the scrollview
    /// </summary>
    public void emptyScrollView(GameObject scrollView) {
        GameManager.clearAllChildrenOfObj(scrollView);
    }

}
