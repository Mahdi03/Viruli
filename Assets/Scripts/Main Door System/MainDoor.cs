using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Doors/Create New Door")]
public class MainDoor : ScriptableObject {

    [Serializable]
    public struct DoorStats {
        public DoorStats(int currentLevel, int maxHealth, int damageDealt) {
            this.currentLevel = currentLevel;
            this.maxHealth = maxHealth;
            this.damageDealt = damageDealt;
            this.doorPrefab = null;
        }
        public int currentLevel;
        public int maxHealth;
        public int damageDealt;
        public GameObject doorPrefab;
    }

    public int ID {
        get; set; //Allow settable for DatabaseManager
    }

    public string doorName { get => this.name; }

    [SerializeField]
    private bool bigDoor; //Use this just as a reminder to me when I'm setting the data

    [SerializeField]
    private string parentTransformTagName;
    private Transform parentTransform; //Where to spawn the door

    //public int initialHealth; //Have small doors have lesser health than the big doors
    [Serializable]
    public class recipeItem {
        public Item item; //keep as Item in case we want to add potions to this
        public int countRequired;
    }

    public GameObject twoDimensionalPrefab;

    public recipeItem[] repairRecipeDirty; //Set through scriptable objects - make public so InGameItemsDatabaseManager can access it
    public List<(int, int)> repairRecipe { get; set; } //Actual recipe object that will be referenced throughout all scripts (set in InGameItemsDatabaseManager.cs)
    public recipeItem[] upgradeToLevel2RecipeDirty; //Set through scriptable objects - make public so InGameItemsDatabaseManager can access it
    public List<(int, int)> upgradeToLevel2Recipe { get; set; } //Actual recipe object that will be referenced throughout all scripts (set in InGameItemsDatabaseManager.cs)

    public int xpToUpgradeToLevel2;

    public recipeItem[] upgradeToLevel3RecipeDirty; //Set through scriptable objects - make public so InGameItemsDatabaseManager can access it
    public List<(int, int)> upgradeToLevel3Recipe { get; set; } //Actual recipe object that will be referenced throughout all scripts (set in InGameItemsDatabaseManager.cs)
    public int xpToUpgradeToLevel3;


    //Door upgrade level stats
    public List<DoorStats> doorStatsAtDifferentUpgradeLevels = new List<DoorStats>() {
            new DoorStats(currentLevel: 1, 100, 0),
            new DoorStats(currentLevel: 2, 100 + 100, 0),
            new DoorStats(currentLevel: 3, 100 + 100 + 200, 4)
        }; //Index of list designates level

    public void InitDoor() {
        //Set the parent transform that we will use throughout the game to access the physical door within the scene
        parentTransform = GameObject.FindGameObjectWithTag(parentTransformTagName).transform;
        spawnDoor();
        //Debug.Log("We spawned");
    }
    private void spawnDoor(int level = 1) {
        GameManager.clearAllChildrenOfObj(parentTransform);
        
        DoorStats doorStats = doorStatsAtDifferentUpgradeLevels[level - 1];
        var newDoor = Instantiate(doorStats.doorPrefab, parentTransform);
        MainDoorController doorController = getDoorController(newDoor.transform);
        doorController.initStats(bigDoor, level, 4f, doorStats.maxHealth, doorStats.damageDealt, this.twoDimensionalPrefab); //Pass along values to the door controller
    }

    public void RepairDoor(int xpCost, int repairCostScale) {
        //Spend upgrade costs
        //Spend XP
        XPSystem.Instance.decreaseXP(xpCost);
        //Use up items
        foreach (var item in this.repairRecipe) { //No need to pass the repair recipe from the UI controller since the UI controller gets it from us
            var id = item.Item1; //Get the recipe item ID
            var countRequired = item.Item2 * repairCostScale; //Get the count of the recipe item
            InventoryManager.Instance.removeByID(id, countRequired); //Remove that much
        }
        //Update UI
        InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
        this.getDoorController().Repair();
    }

    public void UpgradeDoor() {
        int currentDoorLevel = getDoorController().Level;
        if (currentDoorLevel + 1 >= doorStatsAtDifferentUpgradeLevels.Count) {
            Debug.LogError("Surpassed upgradable amount");
            return;
        }
        Debug.Log("We made it here");

        //Get upgrade costs
        List<(int, int)> upgradeRecipe = new List<(int, int)>();
        int xpCost = -1; //Get XP Cost of each upgrade
        switch (this.getDoorController().Level) {
            case 1:
                //upgrade to level 2 costs
                xpCost = this.xpToUpgradeToLevel2;
                upgradeRecipe = this.upgradeToLevel2Recipe;
                break;
            case 2:
                //upgrade to level 3 costs
                xpCost = this.xpToUpgradeToLevel3;
                upgradeRecipe = this.upgradeToLevel3Recipe;
                break;
            default: break;
        }
        //Spend upgrade costs
        XPSystem.Instance.decreaseXP(xpCost);
        foreach (var item in upgradeRecipe) {
            var id = item.Item1; //Get the recipe item ID
            var countRequired = item.Item2; //Get the count of the recipe item
            InventoryManager.Instance.removeByID(id, countRequired); //Remove that much
        }
        InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
        //else we can upgrade the door
        spawnDoor(currentDoorLevel + 1); //Free repair taken care of by completely restarting a new door with a new max health

    }

    public MainDoorController getDoorController(Transform t = null) {
        if (t == null) { t = parentTransform; }
        return t.GetComponentInChildren<MainDoorController>(); //Remember this could be null so be sure to check
    }
    
}
