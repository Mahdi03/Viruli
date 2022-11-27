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
    private int currentDoorLevel = 0;

    [SerializeField]
    private string parentTransformTagName;
    private Transform parentTransform; //Where to spawn the door

    //public int initialHealth; //Have small doors have lesser health than the big doors
    [Serializable]
    public class recipeItem {
        public Item item;
        public int countRequired;
    }

    public recipeItem[] repairRecipeDirty; //Set through scriptable objects - make public so InGameItemsDatabaseManager can access it
    public List<(int, int)> repairRecipe { get; set; } //Actual recipe object that will be referenced throughout all scripts (set in InGameItemsDatabaseManager.cs)
    public recipeItem[] upgradeToLevel2RecipeDirty; //Set through scriptable objects - make public so InGameItemsDatabaseManager can access it
    public List<(int, int)> upgradeToLevel2Recipe { get; set; } //Actual recipe object that will be referenced throughout all scripts (set in InGameItemsDatabaseManager.cs)
    public recipeItem[] upgradeToLevel3RecipeDirty; //Set through scriptable objects - make public so InGameItemsDatabaseManager can access it
    public List<(int, int)> upgradeToLevel3Recipe { get; set; } //Actual recipe object that will be referenced throughout all scripts (set in InGameItemsDatabaseManager.cs)


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
    private void spawnDoor() {
        GameManager.clearAllChildrenOfObj(parentTransform);
        DoorStats doorStats = doorStatsAtDifferentUpgradeLevels[currentDoorLevel];
        var newDoor = Instantiate(doorStats.doorPrefab, parentTransform);
        MainDoorController doorController;
        if (bigDoor) {
            //Door controller is directly attached
            doorController = newDoor.GetComponent<MainDoorController>();
        }
        else {
            //Door contrller is attached to door which is not a direct child
            doorController = newDoor.GetComponentInChildren<MainDoorController>();
        }
        doorController.initStats(bigDoor, doorStats.currentLevel, 4f, doorStats.maxHealth, doorStats.damageDealt); //Pass along values to the door controller
    }

    public void RepairDoor() {
        this.getDoorController().Repair();
    }

    public void UpgradeDoor() {
        if (currentDoorLevel + 1 >= doorStatsAtDifferentUpgradeLevels.Count) {
            Debug.LogError("Surpassed upgradable amount");
            return;
        }

        //else we can upgrade the door
        currentDoorLevel++;
        spawnDoor(); //Free repair taken care of by completely restarting a new door with a new max health

    }

    public MainDoorController getDoorController() {
        return parentTransform.GetComponentInChildren<MainDoorController>(); //Remember this could be null so be sure to check
    }
    
}
