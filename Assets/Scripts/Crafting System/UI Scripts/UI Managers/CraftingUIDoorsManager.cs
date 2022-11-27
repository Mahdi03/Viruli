using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIDoorsManager : MonoBehaviour {
    private static CraftingUIDoorsManager instance;
    public static CraftingUIDoorsManager Instance { get { return instance; } }

    //Bottom half UI containers for tab #2
    [SerializeField]
    private GameObject DoorRepairCenterContainer_BottomLeftCorner;
    [SerializeField]
    private GameObject DoorUpgradeCenterContainer_BottomRightCorner;


    [SerializeField]
    private GameObject healthBarPrefab;


    public int doorID { get; set; }
    private bool doorRepairable { get; set; } = false; //private variable referring to whether a door is repairable
    private bool doorUpgradable { get; set; } = false; //private variable referring to whether a door is upgradable



    [SerializeField]
    private Color tableBorderColor = Color.white;
    [SerializeField]
    private Color tablePaddingColor = Color.HSVToRGB(213 / 360f, 17 / 100f, 21 / 100f);

    private void Awake() {
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }

    }




    //Called every time a door scroll view element is clicked
    public void LoadDoorUI() {
        clearAllDoorInfo();
        ShowDoorRepairUI();
        ShowDoorUpgradeUI();
    }



    private void clearAllDoorInfo() {
        GameManager.clearAllChildrenOfObj(DoorRepairCenterContainer_BottomLeftCorner);
        GameManager.clearAllChildrenOfObj(DoorUpgradeCenterContainer_BottomRightCorner);
    }

    private void ShowDoorRepairUI() {
        //doorID was set from the OnClick Handler of the scrollview element so we can use it here

        var container = new GameObject("Vertical Layout Container");
        VerticalLayoutGroup verticalLayoutGroup = container.AddComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
        verticalLayoutGroup.childControlWidth = true;
        verticalLayoutGroup.childControlHeight = true; //Keep elements on screen

        var containerRectTransform = container.GetComponent<RectTransform>();

        //Sets to stretch
        containerRectTransform.anchorMin = new Vector2(0, 0);
        containerRectTransform.anchorMax = new Vector2(1, 1);
        //Stretch components: https://stackoverflow.com/questions/30782829/how-to-access-recttransforms-left-right-top-bottom-positions-via-code
        //Give 10px margin on left and right
        containerRectTransform.offsetMin = new Vector2(10, 0); //(Left, Bottom)
        containerRectTransform.offsetMax = new Vector2(-10, -0); //(-Right, -Top)

        containerRectTransform.SetParent(DoorRepairCenterContainer_BottomLeftCorner.transform, false);

        //TODO: Show text title "Repair _____ Door:"
        var titleTextbox = new GameObject("Title"); //We don't need to be worried about copies since the entire container is cleared on click anyways

        var titleTextboxText = titleTextbox.AddComponent<TextMeshProUGUI>();

        //Set font size to 16
        titleTextboxText.fontSize = 23f;
        titleTextboxText.verticalAlignment = VerticalAlignmentOptions.Middle;

        var titleTextboxRectTransform = titleTextbox.GetComponent<RectTransform>();
        titleTextboxRectTransform.SetParent(container.transform, false);

        titleTextboxText.text = "Repair " + getDoorName(this.doorID) + " (" + repeatStringNTimes("I", getDoorLevel(this.doorID)) + "):";
        //TODO: Show recipe table (should update as door health changes)
        doorRepairRecipeTable = Table.createNewTable(containerRectTransform, 220, 100);
        UpdateDoorRepairRecipeTable(containerRectTransform); //This will take care of filling in the table for the first time
        //TODO: Show XP requirement

        //TODO: Repair button (disabled if not repairable or at max health)

    }
    private GameObject doorRepairLifeBarContainer;
    private GameObject doorRepairRecipeTable;
    private void UpdateDoorRepairRecipeTable(Transform parentContainerToSpawnElementsIn) {
        var mainDoor = InGameItemsDatabaseManager.Instance.mainDoors[doorID];
        var doorController = mainDoor.getDoorController();
        (int currentDoorHealth, int maxDoorHealth) = doorController.getCurrentHealthStats();
        if (doorRepairLifeBarContainer != null) {
            //Change the bar size and health stats instead of deleting and reinstantiating
        }
        else {
            //We have to instantiate it and then set the values


            doorRepairLifeBarContainer = new GameObject("Door Life Bar Container");

            var doorRepairLifeBarContainerHorizontalLayout = doorRepairLifeBarContainer.AddComponent<HorizontalLayoutGroup>();

            var doorRepairLifeBarContainerRectTransform = doorRepairLifeBarContainer.GetComponent<RectTransform>();
            doorRepairLifeBarContainerRectTransform.SetParent(parentContainerToSpawnElementsIn, false);
            var healthBar = Instantiate(healthBarPrefab, doorRepairLifeBarContainerRectTransform);
            var healthBarController = healthBar.GetComponent<HealthBarBehavior>();
            healthBarController.regularHealthBar = false;
            healthBarController.UpdateHealthBar(currentDoorHealth, maxDoorHealth);

            //Now add text
            var textbox = new GameObject("Health Text");
            var textboxText = textbox.AddComponent<TextMeshProUGUI>();
            Color lerpColor = Color.Lerp(Color.red, Color.green, (float)currentDoorHealth / maxDoorHealth / 100);
            string colorStr = "#" + ColorUtility.ToHtmlStringRGB(lerpColor);
            textboxText.text = "<color=\"" + colorStr + "\">" + currentDoorHealth + "</color>/" + maxDoorHealth;

        }

        GameManager.clearAllChildrenOfObj(this.doorRepairRecipeTable);
        
        var arrOfRecipeItems = mainDoor.repairRecipe;
        this.doorRepairable = true;
        foreach (var item in arrOfRecipeItems) {
            var id = item.Item1;
            var requiredItem = InGameItemsDatabaseManager.Instance.getItemByID(id);
            var countRequired = item.Item2;
            var countAvailable = InventoryManager.Instance.getItemCountByID(id);
            //Make a row
            var row = Table.createTableRow(this.doorRepairRecipeTable.transform, 30f);
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
                this.doorRepairable = false;
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
        var repairXPRequiredTextbox = new GameObject("Required XP");

        TextMeshProUGUI xpText = repairXPRequiredTextbox.AddComponent<TextMeshProUGUI>();

        //Set font size to 16
        xpText.fontSize = 16f;
        xpText.verticalAlignment = VerticalAlignmentOptions.Middle;

        var xpTextRectTransform = repairXPRequiredTextbox.GetComponent<RectTransform>();
        xpTextRectTransform.SetParent(parentContainerToSpawnElementsIn, false);
        
        int currentLevel = doorController.getLevel();
        

        int xpCost = 0; //TODO: Create formula of xpCost for repair based on mainDoors.Level and difference in health

        xpText.text = "XP: <color=\"" + ((XPSystem.Instance.XP < xpCost) ? "red" : "green") + "\">" + XPSystem.Instance.XP + "</color>/" + xpCost;

        if (xpCost > XPSystem.Instance.XP) {
            this.doorRepairable = false;
        }
        //TODO: add button that will be enabled or disabled depending on repairable
        var buttonGameObject = createDefaultButton(parentContainerToSpawnElementsIn, "Repair");
        Button buttonButton = buttonGameObject.GetComponent<Button>();
        buttonButton.enabled = this.doorRepairable; //Enable button depending on whether we can repair
        buttonButton.onClick.AddListener(RepairDoor);
    }

    private void ShowDoorUpgradeUI() {
        //doorID was set from the OnClick Handler of the scrollview element so we can use it here
        var container = new GameObject("Vertical Layout Container");
        VerticalLayoutGroup verticalLayoutGroup = container.AddComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
        verticalLayoutGroup.childControlWidth = true;
        verticalLayoutGroup.childControlHeight = true; //Keep elements on screen

        var containerRectTransform = container.GetComponent<RectTransform>();

        //Sets to stretch
        containerRectTransform.anchorMin = new Vector2(0, 0);
        containerRectTransform.anchorMax = new Vector2(1, 1);
        //Stretch components: https://stackoverflow.com/questions/30782829/how-to-access-recttransforms-left-right-top-bottom-positions-via-code
        //Give 10px margin on left and right
        containerRectTransform.offsetMin = new Vector2(10, 0); //(Left, Bottom)
        containerRectTransform.offsetMax = new Vector2(-10, -0); //(-Right, -Top)

        containerRectTransform.SetParent(DoorUpgradeCenterContainer_BottomRightCorner.transform, false);

        //TODO: Show text title "Upgrade ______ Door:"
        var titleTextbox = new GameObject("Title"); //We don't need to be worried about copies since the entire container is cleared on click anyways

        var titleTextboxText = titleTextbox.AddComponent<TextMeshProUGUI>();

        //Set font size to 16
        titleTextboxText.fontSize = 23f;
        titleTextboxText.verticalAlignment = VerticalAlignmentOptions.Middle;

        var titleTextboxRectTransform = titleTextbox.GetComponent<RectTransform>();
        titleTextboxRectTransform.SetParent(containerRectTransform, false);

        titleTextboxText.text = "Upgrade " + getDoorName(this.doorID) + "(" + repeatStringNTimes("I", getDoorLevel(this.doorID)) + "):";
        //TODO: Show recipe table for upgrade (text if max level reached)
        var mainDoor = InGameItemsDatabaseManager.Instance.mainDoors[doorID];
        int doorLevel = mainDoor.getDoorController().Level;
        int numberOfTotalDoorUpgradableLevels = mainDoor.doorStatsAtDifferentUpgradeLevels.Count;

        bool alreadyUpgradedToMaxLevel = (doorLevel >= numberOfTotalDoorUpgradableLevels);
        if (alreadyUpgradedToMaxLevel) {
            //TODO: Show wall stats and that's it

            var arrOfRecipeItems = mainDoor.repairRecipe;

            var descriptionTextbox = new GameObject("Title"); //We don't need to be worried about copies since the entire container is cleared on click anyways

            var descriptionTextboxText = titleTextbox.AddComponent<TextMeshProUGUI>();

            //Set font size to 16
            descriptionTextboxText.fontSize = 16f;
            descriptionTextboxText.verticalAlignment = VerticalAlignmentOptions.Middle;

            var descriptionTextboxRectTransform = descriptionTextbox.GetComponent<RectTransform>();
            titleTextboxRectTransform.SetParent(containerRectTransform, false);

            descriptionTextboxText.text = getDoorName(this.doorID) + " (Level " + getDoorLevel(this.doorID) + ")" + " is already upgraded to the maximum level";
        }
        else {
            //We can show the table for the next upgrade stuff

            doorUpgradeRecipeTable = Table.createNewTable(containerRectTransform, 220, 100);
            UpdateDoorUpgradeRecipeTable(containerRectTransform);


            //TODO: Show XP requirement
            var upgradeXPRequiredTextbox = new GameObject("Required XP");

            TextMeshProUGUI xpText = upgradeXPRequiredTextbox.AddComponent<TextMeshProUGUI>();

            //Set font size to 16
            xpText.fontSize = 16f;
            xpText.verticalAlignment = VerticalAlignmentOptions.Middle;

            var xpTextRectTransform = upgradeXPRequiredTextbox.GetComponent<RectTransform>();
            xpTextRectTransform.SetParent(containerRectTransform, false);

            int xpCost = 0; //TODO: Get XP Cost of each upgrade

            xpText.text = "XP: <color=\"" + ((XPSystem.Instance.XP < xpCost) ? "red" : "green") + "\">" + XPSystem.Instance.XP + "</color>/" + xpCost;

            if (xpCost > XPSystem.Instance.XP) {
                this.doorUpgradable = false;
            }
            //TODO: Upgrade button (disabled if not upgradable or max level)
            var buttonGameObject = createDefaultButton(containerRectTransform, "Upgrade");
            Button buttonButton = buttonGameObject.GetComponent<Button>();
            buttonButton.enabled = this.doorUpgradable; //Enable button depending on whether we can repair
            buttonButton.onClick.AddListener(UpgradeDoor);
        }


    }



    private GameObject doorUpgradeRecipeTable;
    private void UpdateDoorUpgradeRecipeTable(Transform parentContainerToSpawnElementsIn) {
        GameManager.clearAllChildrenOfObj(doorUpgradeRecipeTable);
        //TODO: implement table????

    }



    private string getDoorName(int id) {
        return InGameItemsDatabaseManager.Instance.mainDoors[id].doorName;
    }
    private int getDoorLevel(int id) {
        return InGameItemsDatabaseManager.Instance.mainDoors[id].getDoorController().Level;
    }
    private string repeatStringNTimes(string str, int n) {
        return string.Concat(Enumerable.Repeat(str, n));
    }
    public void RepairDoor() {
        Debug.Log("Button Clicked"); //On click works
        if (this.doorRepairable) {
            InGameItemsDatabaseManager.Instance.mainDoors[this.doorID].RepairDoor();
        }
    }
    public void UpgradeDoor() {
        if (this.doorUpgradable) {
            InGameItemsDatabaseManager.Instance.mainDoors[this.doorID].UpgradeDoor();
        }
    }

    private GameObject createDefaultButton(Transform parent, string setText = "Default Button") {
        TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources();
        GameObject button = TMP_DefaultControls.CreateButton(resources);
        var buttonRectTransform = button.GetComponent<RectTransform>();
        buttonRectTransform.SetParent(parent, false);
        buttonRectTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = setText;
        return button;
    }



}
