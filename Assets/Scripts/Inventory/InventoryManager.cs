using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
	public static InventoryManager instance;
	//This class set to control both foreground and background inventory?
	[SerializeField]
	static private List<Item> itemsDictionary;

	static private Inventory currentInventory;

	private void Awake() {
		if (instance != this && instance != null) {
			Destroy(gameObject);
		}
		else {
			instance = this;
			//Now we can instantiate stuff
			//currentInventory.loadInventoryFromPlayerPrefs(); //Load in inventory that is already saved on device if exists
		}
	}

}
