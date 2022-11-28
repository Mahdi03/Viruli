using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameItemsDatabaseManager : MonoBehaviour {
	private static InGameItemsDatabaseManager instance;
	public static InGameItemsDatabaseManager Instance { get { return instance; } }

	[SerializeField] //Make it editable but still private to this class only
	private GameItemsDatabase db; //Create scriptable object and then add this through the editor
	private Dictionary<int, IItem> itemsDatabase; //Actual database that will store all the types of items in the game
	public Dictionary<int, IItem> craftableItems { get; private set; }

	public List<int> droppableItems { get; private set; } //Store itemID's a weighted amount for randomized drops

	public List<Enemy> enemies { get; private set; }

	public List<MainDoor> mainDoors { get; private set; }

	//Provide a getter method to get items from the dictionary so we can use the data
	/*
	 Make sure you are checking the value of itemID before trying to access it
	 */
	public IItem getItemByID(int itemID) {
		IItem item;
		if (!itemsDatabase.TryGetValue(itemID, out item)) {
			throw new System.IndexOutOfRangeException("Item with ID: " + itemID + " does not exist");
		}
		return item;
	}


	private void Awake() {
		//Singleton initialization code
		if (instance != this && instance != null) {
			Destroy(gameObject);
		}
		else {
			instance = this;
			//Now we can initialize stuff
			itemsDatabase = new Dictionary<int, IItem>();
			/*
			 * Using TryAdd will try to add it to the dictionary if the key does not already exist
			 * If the key already exists, it will simply do nothing and return false
			 * Use false value to error handle
			 */

			//Also automatically provide ID's here instead of having to manually assign in each scriptable object
			int itemID = 0;
			foreach (var potion in db.potions) {
				potion.ID = itemID; //Set ID in here just in case we need access to it from the actual object
				if (!itemsDatabase.TryAdd(itemID, potion)) {
					throw new System.Exception("An item with this key already exists in the database");
				}
				itemID++;
			}
			foreach (var rawMaterial in db.buildingMaterials) {
				rawMaterial.ID = itemID; //Set ID in here just in case we need access to it from the actual object
				if (!itemsDatabase.TryAdd(itemID, rawMaterial)) {
					throw new System.Exception("An item with this key already exists in the database");
				}
				itemID++;
			}
            foreach (var rawMaterial in db.craftingMaterials) {
                rawMaterial.ID = itemID; //Set ID in here just in case we need access to it from the actual object
                if (!itemsDatabase.TryAdd(itemID, rawMaterial)) {
                    throw new System.Exception("An item with this key already exists in the database");
                }
                itemID++;
            }


            /*
			 * Loop through the dictionary of items and then set each Recipe property as an array of itemIDs
			 * and countRequired's instead of actual Item objects and their counts because the Unity editor
			 * is limited (use .Recipe in script as it is in the form of List<(int, int)>())
			 */
            craftableItems = new Dictionary<int, IItem>();
			droppableItems = new List<int>();
			foreach (KeyValuePair<int, IItem> itemEntry in itemsDatabase) {
				//Debug.Log(itemEntry);
				if (itemEntry.Value.Craftable) {
					//Then we set the recipe correctly
					List<(int, int)> finalRecipe = new List<(int, int)>();

					IItem item = itemEntry.Value;
					var dirtyRecipeItems = item.dirtyRecipe;
					for (int i = 0; i < dirtyRecipeItems.Length; i++) {
						int id = dirtyRecipeItems[i].item.ID;
						int count = dirtyRecipeItems[i].countRequired;
						finalRecipe.Add((id, count));
					}
					item.Recipe = finalRecipe;
					craftableItems.TryAdd(itemEntry.Key, item);
				}

				//Add to probability list (keep in same foreach loop to avoid extra O(n) runtime
				for (int i = 0; i < itemEntry.Value.WeightedDropProbability; i++) {
					droppableItems.Add(itemEntry.Key); //Add itemID as many times as needed by the WeightedDropProbability
				}
			}
			//Set public enemies list from database file
			enemies = db.enemies;
            //Set public mainDoors list from database file and set each of their ID's for later use in the game
			mainDoors = db.mainDoors;
            for (int i = 0; i < db.mainDoors.Count; i++) {
				mainDoors[i].ID = i;
                //Convert dirty repair recipe to clean repair recipe
                List<(int, int)> finalRepairRecipe = new List<(int, int)>();
                var dirtyRepairRecipeItems = mainDoors[i].repairRecipeDirty;
                for (int j = 0; j < dirtyRepairRecipeItems.Length; j++) {
                    int id = dirtyRepairRecipeItems[j].item.ID;
                    int count = dirtyRepairRecipeItems[j].countRequired;
                    finalRepairRecipe.Add((id, count));
                }
                mainDoors[i].repairRecipe = finalRepairRecipe;

                //Convert dirty repair recipe to clean repair recipe
                List<(int, int)> finalUpgradeToLevel2Recipe = new List<(int, int)>();
                var dirtyUpgradeToLevel2RecipeItems = mainDoors[i].upgradeToLevel2RecipeDirty;
                for (int j = 0; j < dirtyUpgradeToLevel2RecipeItems.Length; j++) {
                    int id = dirtyUpgradeToLevel2RecipeItems[j].item.ID;
                    int count = dirtyUpgradeToLevel2RecipeItems[j].countRequired;
                    finalUpgradeToLevel2Recipe.Add((id, count));
                }
                mainDoors[i].upgradeToLevel2Recipe = finalUpgradeToLevel2Recipe;

                //Convert dirty repair recipe to clean repair recipe
                List<(int, int)> finalUpgradeToLevel3Recipe = new List<(int, int)>();
                var dirtyUpgradeToLevel3RecipeItems = mainDoors[i].upgradeToLevel3RecipeDirty;
                for (int j = 0; j < dirtyUpgradeToLevel3RecipeItems.Length; j++) {
                    int id = dirtyUpgradeToLevel3RecipeItems[j].item.ID;
                    int count = dirtyUpgradeToLevel3RecipeItems[j].countRequired;
                    finalUpgradeToLevel3Recipe.Add((id, count));
                }
                mainDoors[i].upgradeToLevel3Recipe = finalUpgradeToLevel3Recipe;


                mainDoors[i].InitDoor();
			}
			
		}
	}

	public void DropRandomItem(Vector3 position, Quaternion rotation) {
		int itemIDToDrop = droppableItems[(int)Random.Range(0, droppableItems.Count)]; //The ID's are numbered off based on count of items total
		getItemByID(itemIDToDrop).drop2DSprite(position, rotation);
	}


}
