using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
	private static InventoryManager instance;
	public static InventoryManager Instance { get { return instance; } }
	//This class set to control both foreground and background inventory?
	public List<Potion> potionList;
	public List<RawMaterial> rawMaterials;
	[SerializeField]
	private List<IItem> mainItemsList; //Use a list of IItems to combine anything that is of type potion or raw material that derives from IItem
	private Dictionary<int, IItem> itemsDatabase;

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
			//mainItemsList = new List<IItem>();
			itemsDatabase = new Dictionary<int, IItem>();
			foreach (var potion in potionList) {
				itemsDatabase.Add(potion.GetItemID(), potion);
				//itemsDictionary.Add(potion);
			}
			foreach (var rawMaterial in rawMaterials) {
				itemsDatabase.Add(rawMaterial.GetItemID(), rawMaterial);
			}
			
			itemsDatabase[0].drop2DSprite(new Vector2(0, 0), Quaternion.identity);
            //itemsDictionary[0].drop2DSprite(new Vector2(30, 10), Quaternion.identity);
        }
	}

	public void pickupItem(IItem item) {
		currentInventory.push(item); //This will take care of putting it in the right place whether or not it is stackable
		/*
		var myPrefab = item.Get2DPrefab();
		Destroy(item.Get2DPrefab());
		item.SetCurrent2DPrefab(myPrefab);
		*/
		//Update inventory UI to reflect inventory array changes
		UpdateInventoryUIToReflectInternalInventoryChanges();
	}
	public void UpdateInventoryUIToReflectInternalInventoryChanges() {
		//Inventory Slot ID matches with index in Inventory list
		GameObject[] inventorySlots = GameObject.FindGameObjectsWithTag("inventorySlot"); //Find all GameObjects that are inventorySlots
		foreach (GameObject inventorySlot in inventorySlots) {
			InventorySlot inventorySlotAssociatedInfo = inventorySlot.GetComponent<InventorySlot>();
            inventorySlotAssociatedInfo.ClearAllItemsInSlot();

            int inventorySlotID = inventorySlotAssociatedInfo.GetSlotID();
			//Make sure we only try to access values that are within the length of the inventory array
            if (!(inventorySlotID < 0 || inventorySlotID >= currentInventory.length())) {
				//If our inventory slot ID is an acceptable index within our internal inventory list
				//All inventory slots have the first two elements being the count and the background,
				// everything else was deleted
				(IItem currentItem, int count) = currentInventory.at(inventorySlotID);

                if (currentItem.GetItemType() != null) { //Using itemType = null as empty placeholders
					//Add this item to the inventory slot at the right position
					
					GameObject item2DPrefab = Instantiate(currentItem.Get2DPrefab(), inventorySlots[inventorySlotID].gameObject.transform);
                    Item.makeDraggable2D(item2DPrefab);
                    Item.disableClickCollectible2D(item2DPrefab);
                    Item.disableHoverFloat2D(item2DPrefab);
                    item2DPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
					//currentItem.SetCurrent2DPrefab(item2DPrefab);
					inventorySlotAssociatedInfo.SetCount(count);
				}
			}
		}
	}
    public int findIDOfNextOpenInventorySlot() {
        return currentInventory.length(); //TODO: Fix because there might be open spots in the middle
    }
}
