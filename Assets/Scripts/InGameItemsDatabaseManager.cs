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
	public IItem getItemByID(int itemID) {
		IItem item;
		itemsDatabase.TryGetValue(itemID, out item);
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
            foreach (var potion in db.potions) {
                itemsDatabase.Add(potion.GetItemID(), potion);
            }
            foreach (var rawMaterial in db.rawMaterials) {
                itemsDatabase.Add(rawMaterial.GetItemID(), rawMaterial);
            }
        }
	}
}
