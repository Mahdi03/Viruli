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
        var xpCost = InGameItemsDatabaseManager.Instance.getItemByID(itemID).XPCost * amountToCraft; //Don't forget to factor in the amount they are trying to make
        XPSystem.Instance.decreaseXP(xpCost);

        //Then add it to the inventory in the next convenient location
        for (int i = 0; i < amountToCraft; i++) {
            //Use already made pick up item but don't gain XP from crafting stuff (it's too easy of a hack)
            InventoryManager.Instance.pickupItem(itemID, disableXPIncrease: true); //This will automatically also refresh our inventory UI to reflect the new changes
        }
    }
}
