using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    private GameObject sliderPrefab;


    public int doorID { get; set; }
    private bool doorRepairable { get; set; } = false; //private variable referring to whether a door is repairable
    private bool doorUpgradable { get; set; } = false; //private variable referring to whether a door is upgradable


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
        /*foreach (GameObject g in new[] { doorRepairLifeBarContainer, doorRepairXPRequiredTextbox, doorRepairButtonGameObject, doorRepairRecipeTable }) {
            Destroy(g);
        }*/

        if (doorRepairTableUpdateCoroutine != null) {
            StopCoroutine(doorRepairTableUpdateCoroutine);
            doorRepairTableUpdateCoroutine = null;
        }
        GameManager.clearAllChildrenOfObj(DoorRepairCenterContainer_BottomLeftCorner);
        GameManager.clearAllChildrenOfObj(DoorUpgradeCenterContainer_BottomRightCorner);
    }

    /***********************************************************Door Repair**************************************************************/


    //TODO: fix glitch in UI where it appears lower for a split second before it recenters (I suspect it's the update function)
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

        //Show text title "Repair _____ Door:"
        var titleTextbox = new GameObject("Title"); //We don't need to be worried about copies since the entire container is cleared on click anyways

        var titleTextboxText = titleTextbox.AddComponent<TextMeshProUGUI>();

        //Set font size to 16
        titleTextboxText.fontSize = 23f;
        titleTextboxText.verticalAlignment = VerticalAlignmentOptions.Middle;

        var titleTextboxRectTransform = titleTextbox.GetComponent<RectTransform>();
        titleTextboxRectTransform.SetParent(container.transform, false);

        titleTextboxText.text = "Repair " + getDoorName(this.doorID) + " (" + repeatStringNTimes("I", getDoorLevel(this.doorID)) + "):";
        //Show recipe table (should update as door health changes)
        doorRepairRecipeTable = Table.createNewTable(containerRectTransform, 220, 100);
        if (doorRepairTableUpdateCoroutine != null) {
            StopCoroutine(doorRepairTableUpdateCoroutine);
            doorRepairTableUpdateCoroutine = null;
        }
        doorRepairTableUpdateCoroutine = UpdateDoorRepairRecipeTable(containerRectTransform); //This will take care of filling in the table for the first time
        StartCoroutine(doorRepairTableUpdateCoroutine);

    }
    //Use this method to resume our update coroutine if we close the crafting ui and then reopen it
    public void resumeCoroutines() {
        if (doorRepairTableUpdateCoroutine != null) {
            StartCoroutine(doorRepairTableUpdateCoroutine);
        }
    }
    private GameObject doorRepairLifeBarContainer, doorRepairXPRequiredTextbox, doorRepairButtonGameObject;
    private GameObject doorRepairRecipeTable;
    private IEnumerator doorRepairTableUpdateCoroutine;
    private IEnumerator UpdateDoorRepairRecipeTable(Transform parentContainerToSpawnElementsIn) {
        var mainDoor = InGameItemsDatabaseManager.Instance.mainDoors[doorID];
        var doorController = mainDoor.getDoorController();
        (int currentDoorHealth, int maxDoorHealth) = doorController.getCurrentHealthStats();


        GameObject healthBar;
        TextMeshProUGUI healthTextboxText;
        if (doorRepairLifeBarContainer == null) {
            //Then we need to instantiate it
            doorRepairLifeBarContainer = new GameObject("Life Bar Container");

            var doorRepairLifeBarContainerHorizontalLayout = doorRepairLifeBarContainer.AddComponent<HorizontalLayoutGroup>();
            doorRepairLifeBarContainerHorizontalLayout.childControlWidth = false;
            doorRepairLifeBarContainerHorizontalLayout.childControlHeight = false;
            doorRepairLifeBarContainerHorizontalLayout.childAlignment = TextAnchor.MiddleLeft;
            doorRepairLifeBarContainerHorizontalLayout.childForceExpandWidth = false;
            doorRepairLifeBarContainerHorizontalLayout.childForceExpandHeight = false;
            doorRepairLifeBarContainerHorizontalLayout.childScaleWidth = true;
            doorRepairLifeBarContainerHorizontalLayout.childScaleHeight = true;
            doorRepairLifeBarContainerHorizontalLayout.spacing = 5;

            var doorRepairLifeBarContainerRectTransform = doorRepairLifeBarContainer.GetComponent<RectTransform>();
            doorRepairLifeBarContainerRectTransform.SetParent(parentContainerToSpawnElementsIn, false);
            healthBar = Instantiate(sliderPrefab, doorRepairLifeBarContainerRectTransform);

            var healthTextbox = new GameObject("Health Text");
            healthTextboxText = healthTextbox.AddComponent<TextMeshProUGUI>();

            RectTransform textboxRectTransform = healthTextbox.GetComponent<RectTransform>();
            textboxRectTransform.SetParent(doorRepairLifeBarContainerRectTransform);

        }
        else {
            healthBar = doorRepairLifeBarContainer.transform.GetChild(0).gameObject; //The bar is the first child
            var healthTextbox = doorRepairLifeBarContainer.transform.GetChild(1).gameObject; //The accompanying text is the second child
            healthTextboxText = healthTextbox.GetComponent<TextMeshProUGUI>();
        }

        //Now we set the values
        var slider = healthBar.GetComponent<Slider>();
        slider.maxValue = maxDoorHealth;
        slider.value = currentDoorHealth;
        Color lerpColor = Color.Lerp(Color.red, Color.green, slider.normalizedValue);
        slider.fillRect.GetComponentInChildren<Image>().color = lerpColor;

        //Now add text
        string colorStr = "#" + ColorUtility.ToHtmlStringRGB(lerpColor);
        healthTextboxText.text = "<color=" + colorStr + ">" + currentDoorHealth + "</color>/" + maxDoorHealth;


        GameManager.clearAllChildrenOfObj(this.doorRepairRecipeTable);

        var arrOfRecipeItems = mainDoor.repairRecipe;
        this.doorRepairable = true;

        bool recipeRequirementsMet = CraftingUIController.fillOutRecipeTable(this.doorRepairRecipeTable, arrOfRecipeItems);
        if (!recipeRequirementsMet ) { this.doorRepairable = false; } //Only set it to false if we failed to meet the requirements
        
        //Show XP required
        TextMeshProUGUI xpText;
        if (doorRepairXPRequiredTextbox == null) {
            doorRepairXPRequiredTextbox = new GameObject("Required XP");

            xpText = doorRepairXPRequiredTextbox.AddComponent<TextMeshProUGUI>();

            //Set font size to 16
            xpText.fontSize = 16f;
            xpText.verticalAlignment = VerticalAlignmentOptions.Middle;

            var xpTextRectTransform = doorRepairXPRequiredTextbox.GetComponent<RectTransform>();
            xpTextRectTransform.SetParent(parentContainerToSpawnElementsIn, false);
        }
        else {
            xpText = doorRepairXPRequiredTextbox.GetComponent<TextMeshProUGUI>();
        }
        int currentLevel = doorController.getLevel();
        int xpCost = 0; //TODO: Create formula of xpCost for repair based on mainDoors.Level and difference in health

        xpText.text = "XP: <color=\"" + ((XPSystem.Instance.XP < xpCost) ? "red" : "green") + "\">" + XPSystem.Instance.XP + "</color>/" + xpCost;

        if (xpCost > XPSystem.Instance.XP) {
            this.doorRepairable = false;
        }
        //add button that will be enabled or disabled depending on repairable
        if (this.doorRepairButtonGameObject == null) {
            doorRepairButtonGameObject = createDefaultButton(parentContainerToSpawnElementsIn, "Repair");
            Button buttonButton = doorRepairButtonGameObject.GetComponent<Button>();
            buttonButton.enabled = this.doorRepairable; //Enable button depending on whether we can repair
            buttonButton.onClick.AddListener(RepairDoor);
        }
        else {
            Button buttonButton = doorRepairButtonGameObject.GetComponent<Button>();
            buttonButton.enabled = this.doorRepairable; //Enable button depending on whether we can repair
        }

        yield return new WaitForSeconds(Time.smoothDeltaTime); //Update every frame
        doorRepairTableUpdateCoroutine = UpdateDoorRepairRecipeTable(parentContainerToSpawnElementsIn);
        StartCoroutine(doorRepairTableUpdateCoroutine);
        //StartCoroutine("UpdateDoorRepairRecipeTable", parentContainerToSpawnElementsIn); //Use string so that we can 
    }

    /***********************************************************Door Upgrade**************************************************************/

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

        //Show text title "Upgrade ______ Door:"
        var titleTextbox = new GameObject("Title"); //We don't need to be worried about copies since the entire container is cleared on click anyways

        var titleTextboxText = titleTextbox.AddComponent<TextMeshProUGUI>();

        //Set font size to 16
        titleTextboxText.fontSize = 23f;
        titleTextboxText.verticalAlignment = VerticalAlignmentOptions.Middle;

        var titleTextboxRectTransform = titleTextbox.GetComponent<RectTransform>();
        titleTextboxRectTransform.SetParent(containerRectTransform, false);

        titleTextboxText.text = "Upgrade " + getDoorName(this.doorID) + ":";
        //TODO: Show recipe table for upgrade (text if max level reached)
        var mainDoor = InGameItemsDatabaseManager.Instance.mainDoors[doorID];
        int doorLevel = mainDoor.getDoorController().Level;
        int numberOfTotalDoorUpgradableLevels = mainDoor.doorStatsAtDifferentUpgradeLevels.Count;

        bool alreadyUpgradedToMaxLevel = (doorLevel >= numberOfTotalDoorUpgradableLevels);
        if (alreadyUpgradedToMaxLevel) {
            //TODO: Show wall stats and that's it

            var descriptionTextbox = new GameObject("Title"); //We don't need to be worried about copies since the entire container is cleared on click anyways
            var descriptionTextboxText = titleTextbox.AddComponent<TextMeshProUGUI>();

            //Set font size to 16
            descriptionTextboxText.fontSize = 16f;
            descriptionTextboxText.verticalAlignment = VerticalAlignmentOptions.Middle;

            var descriptionTextboxRectTransform = descriptionTextbox.GetComponent<RectTransform>();
            titleTextboxRectTransform.SetParent(containerRectTransform, false);

            descriptionTextboxText.text = getDoorName(this.doorID) + " (" + repeatStringNTimes("I", getDoorLevel(this.doorID)) + ")" + " is already upgraded to the maximum level";
        }
        else {
            //Calculate upgrade costs first
            //Debug.Log(mainDoor.getDoorController().Level);
            List<(int, int)> upgradeRecipe = new List<(int, int)>();
            int xpCost = -1; //Get XP Cost of each upgrade
            switch (mainDoor.getDoorController().Level) {
                case 1:
                    //upgrade to level 2 costs
                    xpCost = mainDoor.xpToUpgradeToLevel2;
                    upgradeRecipe = mainDoor.upgradeToLevel2Recipe;
                    break;
                case 2:
                    //upgrade to level 3 costs
                    xpCost = mainDoor.xpToUpgradeToLevel3;
                    upgradeRecipe= mainDoor.upgradeToLevel3Recipe;
                    break;
                default: break;
            }



            //We can show the table for the next upgrade stuff

            doorUpgradeRecipeTable = Table.createNewTable(containerRectTransform, 220, 100);
            this.doorUpgradable = true; //Start at true (will change as we check all the values)
            UpdateDoorUpgradeRecipeTable(containerRectTransform, upgradeRecipe);


            //Show XP requirement
            var upgradeXPRequiredTextbox = new GameObject("Required XP");

            TextMeshProUGUI xpText = upgradeXPRequiredTextbox.AddComponent<TextMeshProUGUI>();

            //Set font size to 16
            xpText.fontSize = 16f;
            xpText.verticalAlignment = VerticalAlignmentOptions.Middle;

            var xpTextRectTransform = upgradeXPRequiredTextbox.GetComponent<RectTransform>();
            xpTextRectTransform.SetParent(containerRectTransform, false);
            
            xpText.text = "XP: <color=\"" + ((XPSystem.Instance.XP < xpCost) ? "red" : "green") + "\">" + XPSystem.Instance.XP + "</color>/" + xpCost;

            if (xpCost > XPSystem.Instance.XP) {
                this.doorUpgradable = false;
            }
            //Upgrade button (disabled if not upgradable or max level)
            var buttonGameObject = createDefaultButton(containerRectTransform, "Upgrade");
            Button buttonButton = buttonGameObject.GetComponent<Button>();
            buttonButton.enabled = this.doorUpgradable; //Enable button depending on whether we can repair
            buttonButton.onClick.AddListener(UpgradeDoor);
        }


    }



    private GameObject doorUpgradeRecipeTable;
    private void UpdateDoorUpgradeRecipeTable(Transform parentContainerToSpawnElementsIn, List<(int, int)> upgradeRecipe) {
        GameManager.clearAllChildrenOfObj(doorUpgradeRecipeTable);
        //Loop through all items in upgrade recipe and add them to the table
        //TODO: implement table????
        
        bool recipeRequirementsMet = CraftingUIController.fillOutRecipeTable(this.doorUpgradeRecipeTable, upgradeRecipe);
        if (!recipeRequirementsMet) { this.doorUpgradable = false; }

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
