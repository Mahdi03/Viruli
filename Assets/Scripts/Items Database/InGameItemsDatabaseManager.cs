using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameItemsDatabaseManager : MonoBehaviour {
	private static InGameItemsDatabaseManager instance;
	public static InGameItemsDatabaseManager Instance { get { return instance; } }

	[SerializeField] //Make it editable but still private to this class only
	private GameItemsDatabase db; //Create scriptable object and then add this through the editor
	private Dictionary<int, IItem> itemsDatabase;

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
            foreach (var rawMaterial in db.rawMaterials) {
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

			foreach (KeyValuePair<int, IItem> itemEntry in itemsDatabase) {
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
				}
			}
        }
	}
}
