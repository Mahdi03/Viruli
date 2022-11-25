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
			currentInventory = new Inventory(100); //TODO: Have a dynamically resizing inventory depending on how many items we have
			//TODO: Have an if resume game catch here
			currentInventory.loadInventoryFromPlayerPrefs(); //Load in inventory that is already saved on device if exists
			IItem myItem = InGameItemsDatabaseManager.Instance.getItemByID(7);

			myItem.drop2DSprite(new Vector2(0, 0), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(30, 10), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(-30, -10), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(0 - 4, 0 + 49), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(13, 1), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(60, -15), Quaternion.identity);

			myItem = InGameItemsDatabaseManager.Instance.getItemByID(6);

			myItem.drop2DSprite(new Vector2(20, 40), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(30, 0), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(-15, -10), Quaternion.identity);
			myItem = InGameItemsDatabaseManager.Instance.getItemByID(9);
			myItem.drop2DSprite(new Vector2(10, 38), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(90, 15), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(-5, -1), Quaternion.identity);
			myItem = InGameItemsDatabaseManager.Instance.getItemByID(10);
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
		//Add +2*level+3 XP for picking up something
		XPSystem.Instance.increaseXP(2 * XPSystem.Instance.Level + 3); //The amount of XP earned from a pick up will change based on what level you are on
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

		//Loop through every single slot available to us (there will be 10 if the inventory is closed and a lot more if it is open

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
						//Let's loop thru all the elements and make sure to add the prefab to all slots that match the slotID
						foreach (var inventorySlotA in inventorySlots) {
							InventorySlot inventorySlotAssociatedInfoA = inventorySlotA.GetComponent<InventorySlot>();
							if (inventorySlotAssociatedInfoA.slotID == inventorySlotID) {
								GameObject item2DPrefab = Instantiate(currentItem.TwoDimensionalPrefab, inventorySlotA.gameObject.transform);
								IItem.attachItemInstance(item2DPrefab, currentItemID, inventorySlotID);
								IItem.enableScript<DraggableObject2D>(item2DPrefab);
								if (inventorySlotAssociatedInfoA.slotID < 10) {
									IItem.enableScript<DroppableObject3D>(item2DPrefab);
								}
								IItem.disableScript<ClickAddInventory>(item2DPrefab);
								IItem.disableScript<ItemFloat>(item2DPrefab);
								IItem.enableScript<OnHoverTooltip>(item2DPrefab);
								item2DPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
								inventorySlotAssociatedInfoA.Count = count;
							}
						}
					}
				}
				inventorySlotAssociatedInfo.Count = count; //Make sure to update the count for all slots even empty placeholders
			}
		}
	}
}
