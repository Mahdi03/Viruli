using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler {
	public Image container;
	public Image item;
	public GameObject countText;
	private TextMeshProUGUI countTextValue;
	public int slotID { get; set; }
	private (Item, int) inventoryTupleStoredHere;
	public int itemID { get; set; } = -1;
	private int count = 0;
	public int Count {
		get { return count; }
		set {
			count = value;
			countText.SetActive(!(count < 2)); //Disable count text if there is 1 or none
			countTextValue.text = count.ToString();
		}
	}

	private Vector2 currentSlotRectTransformAnchorMin, currentSlotRectTransformAnchorMax, currentSlotRectTransformAnchorPos;


	public void ClearAllItemsInSlot() {
		//Delete items backwards until we reach the second one
		for (int i = transform.childCount - 1; i > 1; i--) {
			GameObject objToDelete = transform.GetChild(i).gameObject;
			Destroy(objToDelete);
		}
	}

	private void Awake() {
		countTextValue = countText.GetComponent<TextMeshProUGUI>();
		countText.SetActive(!(count < 2)); //Disable count text if there is 1 or none
		RectTransform rectTransform = GetComponent<RectTransform>();
		currentSlotRectTransformAnchorMin = rectTransform.anchorMin;
		currentSlotRectTransformAnchorMax = rectTransform.anchorMax;
		currentSlotRectTransformAnchorPos = rectTransform.anchoredPosition;
	}

	public void OnDrop(PointerEventData eventData) {
		GameObject itemDroppedIntoSlot = eventData.pointerDrag; //Get actual object that was dropped on this current object
		if (itemDroppedIntoSlot != null) {
			//Ok we need to check whether we can drop the current object into this slot
			if (transform.childCount > 2) {
				if (transform.childCount != 3) {
					throw new System.Exception("InventorySlot has too many items");
				}
				//An item already exists, get the last child's item's ID and check with this item's ID to add it
				GameObject currentItemAlreadyInSlot = transform.GetChild(transform.childCount - 1).gameObject;
				ItemInstance currentItemAlreadyInSlotInstance = currentItemAlreadyInSlot.GetComponent<ItemInstance>();
				ItemInstance itemDroppedIntoSlotInstance = itemDroppedIntoSlot.GetComponent<ItemInstance>();
				if (currentItemAlreadyInSlotInstance != null && itemDroppedIntoSlotInstance != null) {
					if (currentItemAlreadyInSlotInstance.itemID == itemDroppedIntoSlotInstance.itemID) {
						//Then they are the same object
						/*
						 * TODO
						 * SOOOOO this is called when 2 things happen, either they start dragging and then realize they don't want to
						 * so they drag it back in place (in which case on this drop 
						*/
						//Let's check if it is stackable
						IItem currentItem = InGameItemsDatabaseManager.Instance.getItemByID(currentItemAlreadyInSlotInstance.itemID);
						if (currentItem.Stackable) {
							//Then we can add the current item to this slot, get count of that item and then make that the current text
							//We should merge it in the Inventory class and then just reflect the changes in the UI

							//This method is highly unlikely
							throw new System.NotImplementedException("This is pretty unlikely and I don't see it happening");
						}
						else {
							//They are not stackable so we can swap the two items but that's stupid if it's the same item because they'll be identical, they'll never know
							//So reset UI and destroy that object
							Destroy(itemDroppedIntoSlot);
							InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
						}
					}
					else {
						//Items do not match, we want to swap the two elements
						//Call the internal swap function with the index of the two elements in the inventory and then reflect the changes in the UI
						//The index of the elements should be equal to the index in the inventory array equal to the inventorySlotID
						InventoryManager.Instance.swapItemsInInventory(currentItemAlreadyInSlotInstance.attachedInventorySlotID, itemDroppedIntoSlotInstance.attachedInventorySlotID);
						Destroy(itemDroppedIntoSlot);
					}
				}
				else {
					throw new System.Exception("Could not find ItemInstance scripts on drop");
				}
			}
			else {
				//This slot is empty, if we wanna move it here we should swap this empty slot with the slot where this item is coming from
				ItemInstance itemDroppedIntoSlotInstance = itemDroppedIntoSlot.GetComponent<ItemInstance>();
				InventoryManager.Instance.swapItemsInInventory(slotID, itemDroppedIntoSlotInstance.attachedInventorySlotID);
				Destroy(itemDroppedIntoSlot); //We instantiate a new one inside InventoryManager that is set to the correct position
			}
		}
	}
}
