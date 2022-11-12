using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
	private static InventoryManager instance;
	public static InventoryManager Instance { get { return instance; } }
	//This class set to control both foreground and background inventory?

	private Inventory currentInventory;

	private void Awake() {
		if (instance != this && instance != null) {
			Destroy(gameObject);
		}
		else {
			instance = this;
			//Now we can instantiate stuff
			currentInventory = new Inventory(100);
			//TODO: Have an if resume game catch here
			currentInventory.loadInventoryFromPlayerPrefs(); //Load in inventory that is already saved on device if exists
			IItem myItem = InGameItemsDatabaseManager.Instance.getItemByID(0);
			
			myItem.drop2DSprite(new Vector2(0, 0), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(30, 10), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(-30, -10), Quaternion.identity);

            myItem = InGameItemsDatabaseManager.Instance.getItemByID(1);

            myItem.drop2DSprite(new Vector2(20, 40), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(30, 0), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(-15, -10), Quaternion.identity);
        }
	}

	public void pickupItem(int itemID) {
		currentInventory.Add(itemID); //This will take care of putting it in the right place whether or not it is stackable
		//Update inventory UI to reflect inventory array changes
		UpdateInventoryUIToReflectInternalInventoryChanges();
	}
	public void swapItemsInInventory(int indexA, int indexB) {
		currentInventory.swap(indexA, indexB);
		UpdateInventoryUIToReflectInternalInventoryChanges();
	}
	//This is a hard refresh method for all items in inventory, batch instantiation and destruction is expensive,
	// find a less lazy programming approach
	public void UpdateInventoryUIToReflectInternalInventoryChanges() {
		//Inventory Slot ID matches with index in Inventory list
		GameObject[] inventorySlots = GameObject.FindGameObjectsWithTag("inventorySlot"); //Find all GameObjects that are inventorySlots
		foreach (GameObject inventorySlot in inventorySlots) {
			InventorySlot inventorySlotAssociatedInfo = inventorySlot.GetComponent<InventorySlot>();
			//All inventory slots have the first two elements being the count and the background,
			// everything else is deleted
			inventorySlotAssociatedInfo.ClearAllItemsInSlot();

			int inventorySlotID = inventorySlotAssociatedInfo.slotID;
			//Make sure we only try to access values that are within the length of the inventory array
			//If our inventory slot ID is an acceptable index within our internal inventory list
			if (!(inventorySlotID < 0 || inventorySlotID >= currentInventory.length())) {
				
				(int currentItemID, int count) = currentInventory.at(inventorySlotID);
				if (currentItemID < 0) {
					//This object is an empty placeholder, skip
				}
				else {
					IItem currentItem = InGameItemsDatabaseManager.Instance.getItemByID(currentItemID);
					if (currentItem.itemType != null) { //Using itemType = null as empty placeholders
						//Add this item to the inventory slot at the right position
						GameObject item2DPrefab = Instantiate(currentItem.TwoDimensionalPrefab, inventorySlots[inventorySlotID].gameObject.transform);
						Item.attachItemInstance(item2DPrefab, currentItemID, inventorySlotID);
						Item.makeDraggable2D(item2DPrefab);
						Item.disableClickCollectible2D(item2DPrefab);
						Item.disableHoverFloat2D(item2DPrefab);
						item2DPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
						//currentItem.SetCurrent2DPrefab(item2DPrefab);
						
					}
				}
                inventorySlotAssociatedInfo.Count = count; //Make sure to update the count for all slots even empty placeholders

            }
		}
	}
}
