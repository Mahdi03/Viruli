using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour {
	public void Craft(int itemID, int amountToCraft) {
		//First remove all the item's requirements from the inventory
		var arrOfRecipeItems = InGameItemsDatabaseManager.Instance.getItemByID(itemID).Recipe;
		foreach (var item in arrOfRecipeItems) {
			var id = item.Item1; //Get the recipe item ID
			var countRequired = item.Item2 * amountToCraft; //Get the count of the recipe item
			InventoryManager.Instance.removeByID(id, countRequired); //Remove that much
		}

		//TODO: spend XP to craft
		//Then add it to the inventory in the next convenient location
		InventoryManager.Instance.pickupItem(itemID); //This will automatically also refresh our inventory UI to reflect the new changes
	}
}
