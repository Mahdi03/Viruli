using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
	private static InventoryManager instance;
	public static InventoryManager Instance { get { return instance; } }
	//This class set to control both foreground and background inventory?

	private Inventory currentInventory;

	private void Start() { //This relies on another component to already be initialized so we do that component in Awake and this one in Start
		if (instance != this && instance != null) {
			Destroy(gameObject);
		}
		else {
			instance = this;
			//Now we can instantiate stuff
			currentInventory = new Inventory(100);
			//TODO: Have an if resume game catch here
			currentInventory.loadInventoryFromPlayerPrefs(); //Load in inventory that is already saved on device if exists
			IItem myItem = InGameItemsDatabaseManager.Instance.getItemByID(7);
			
			myItem.drop2DSprite(new Vector2(0, 0), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(30, 10), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(-30, -10), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(0-4, 0+49), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(13, 1), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(60, -15), Quaternion.identity);

            myItem = InGameItemsDatabaseManager.Instance.getItemByID(6);

			myItem.drop2DSprite(new Vector2(20, 40), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(30, 0), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(-15, -10), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(10, 38), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(90, 15), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(-5, -1), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(20, 0), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(3, 0), Quaternion.identity);
            myItem.drop2DSprite(new Vector2(-17, 10), Quaternion.identity);




        }
	}

	public void removeByID(int itemIDToRemove, int amountToRemove = 1) {
		currentInventory.removeByID(itemIDToRemove, amountToRemove);
	}


    public int getTotalItemsCount() {
		return this.currentInventory.length();
	}

	public int getItemCountByID(int id) {
		return currentInventory.getItemCountByID(id);
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
						//We are making two of these here because when the id of the slot is 0, we only instantiate it at 0, not 
						//We need to brute force it I guess
						if (inventorySlots.Length > 10) {
							//We now need to consider the duplicate 
						}
						if ((inventorySlotID >= 0 && inventorySlotID < 10)) {
							/*
							 * When our ID is between 0-9 and we have more than 10 inventory slots on the screen
							 * we can assume that they have the inventory open so we need to instantiate the item
							 * at both places
							 */

						}
						//Let's loop thru all the elements and make sure to add the prefab to all slots that match the slotID
						foreach (var inventorySlotA in inventorySlots) {
							InventorySlot inventorySlotAssociatedInfoA = inventorySlotA.GetComponent<InventorySlot>();
							if (inventorySlotAssociatedInfoA.slotID == inventorySlotID) {
								GameObject item2DPrefab = Instantiate(currentItem.TwoDimensionalPrefab, inventorySlotA.gameObject.transform);
								Item.attachItemInstance(item2DPrefab, currentItemID, inventorySlotID);
								Item.makeDraggable2D(item2DPrefab);
								Item.disableClickCollectible2D(item2DPrefab);
								Item.disableItemFloat2D(item2DPrefab);
								Item.allowHoverTooltip(item2DPrefab);
								item2DPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
								inventorySlotAssociatedInfoA.Count = count;
							}
						}
						//currentItem.SetCurrent2DPrefab(item2DPrefab);
						
					}
				}
				inventorySlotAssociatedInfo.Count = count; //Make sure to update the count for all slots even empty placeholders

			}
		}
	}
}
