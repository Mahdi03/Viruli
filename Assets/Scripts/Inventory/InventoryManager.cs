using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
	private static InventoryManager instance;
	public static InventoryManager Instance { get { return instance; } }
	//This class set to control both foreground and background inventory?

	private Inventory currentInventory;

	private AudioSource itemPickupNoise;

	private void Start() { //This relies on another component to already be initialized so we do that component in Awake and this one in Start
		if (instance != this && instance != null) {
			Destroy(gameObject);
		}
		else {
			instance = this;
			//Now we can instantiate stuff

			itemPickupNoise = GetComponent<AudioSource>();

			currentInventory = new Inventory(100); //TODO: Have a dynamically resizing inventory depending on how many items we have
			
			
			IItem myItem = InGameItemsDatabaseManager.Instance.getItemByID(0); //Item ID:0 is attack potion #1

			myItem.drop2DSprite(new Vector2(0, 0), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(30, 10), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(-30, -10), Quaternion.identity);
			
			myItem.drop2DSprite(new Vector2(0 - 4, 0 + 49), Quaternion.identity);
			myItem.drop2DSprite(new Vector2(13, 1), Quaternion.identity);
			/*
			myItem.drop2DSprite(new Vector2(60, -15), Quaternion.identity);

			myItem = InGameItemsDatabaseManager.Instance.getItemByID(11);

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
			*/
		}
	}

	public void LoadInventoryFromJSONString(string jsonStr) {
		currentInventory.loadFromJSONString(jsonStr);
		this.UpdateInventoryUIToReflectInternalInventoryChanges();
	}
	public void removeByID(int itemIDToRemove, int amountToRemove = 1) {
		currentInventory.removeByID(itemIDToRemove, amountToRemove);
	}
	public void removeAtSlotLocation(int indexToRemoveAt, int amountToRemove = 1) {
		currentInventory.removeAtIndex(indexToRemoveAt, amountToRemove);
	}

	public int getTotalItemsCount() {
		return this.currentInventory.length();
	}

	public int getItemCountByID(int id) {
		return currentInventory.getItemCountByID(id);
	}

	public int getCountOfRemainingOpenSlots() {
		return currentInventory.getCountOfRemainingOpenSpots();
	}

	/// <summary>
	/// Make return type a bool so that we can check whether it was added successfully or not
	/// and based off that make a decision on whether we should remove it from the scene altogether or what
	/// </summary>
	/// <param name="itemID"></param>
	/// <param name="disableXPIncrease"></param>
	/// <returns></returns>
	public bool pickupItem(int itemID, bool disableXPIncrease = false) {
		var item = InGameItemsDatabaseManager.Instance.getItemByID(itemID);
		if (item.Stackable) {
			if (currentInventory.indexOf(itemID) < 0) {
				if (getCountOfRemainingOpenSlots() < 1) {
				//Item not found in inventory, see if we have space to add one more item
					return false; //Do not continue the function, we can't add it to the inventory
				}
			}
		}
		else {
            //Item is not stackable, just see if we have one slot open for this item or not
            if (getCountOfRemainingOpenSlots() < 1) {
                //Item not found in inventory, see if we have space to add one more item
                return false; //Do not continue the function, we can't add it to the inventory
            }
        }
        currentInventory.Add(itemID); //This will take care of putting it in the right place whether or not it is stackable
		itemPickupNoise.Play();
		if (!disableXPIncrease) {
			//Add +2*level+3 XP for picking up something
			XPSystem.Instance.increaseXP(2 * XPSystem.Instance.Level + 3); //The amount of XP earned from a pick up will change based on what level you are on
		}
		//Update inventory UI to reflect inventory array changes
		UpdateInventoryUIToReflectInternalInventoryChanges();
		return true; //Since we made it here, it was a successful add to the inventory
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
								GameObject item2DPrefab = Instantiate(currentItem.TwoDimensionalPrefab, inventorySlotA.gameObject.transform.GetChild(0));
								IItem.attachItemInstance(item2DPrefab, currentItemID, inventorySlotID);
								IItem.enableScript<DraggableObject2D>(item2DPrefab);
								if (inventorySlotAssociatedInfoA.slotID < 10) {
									IItem.enableScript<DroppableObject3D>(item2DPrefab);

                                    IItem.disableScript<ClickAddQuickInventory>(item2DPrefab);
                                }
								else {
                                    //we are in the full inventory

                                    //make it so that when we click on an element (pointer up maybe)
                                    //that it is added to the nearest spot in the quick inventory instead of having to drag it
                                    IItem.enableScript<ClickAddQuickInventory>(item2DPrefab);

                                    IItem.disableScript<DroppableObject3D>(item2DPrefab);
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

	public string GetCurrentInventoryJSONString() {
		return currentInventory.saveToJSONString();
	}
}
