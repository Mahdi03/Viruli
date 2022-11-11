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
    private int slotID;
    private (Item, int) inventoryTupleStoredHere;
    private int itemID = -1;
    private int count = 0;

    private Vector2 currentSlotRectTransformAnchorMin, currentSlotRectTransformAnchorMax, currentSlotRectTransformAnchorPos;

    public void SetSlotID(int id) {
        slotID = id;
    }
    public int GetSlotID() { return slotID; }
    public int GetItemID() { return itemID; }
    public void SetItemID(int id) { itemID = id; }
    public int GetCount() { return count; }
    public void SetCount(int count) {
        this.count = count;
        countText.SetActive(!(count < 2)); //Disable count text if there is 1 or none
        countTextValue.text = count.ToString();
    }
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
                AttachedItemData alreadyInSlotItemData = currentItemAlreadyInSlot.GetComponent<AttachedItemData>();
                AttachedItemData currentItemData = itemDroppedIntoSlot.GetComponent<AttachedItemData>();
                if (alreadyInSlotItemData != null && currentItemData != null) {
                    if (alreadyInSlotItemData.itemID == currentItemData.itemID) {
                        //Then we can add the current item to inventory, get count of that item and then make that the current text
                    }
                    else {
                        //Items do not match, we want to place the new item at this position of the array and then add the other item to the nearest empty spot or the end of the line
                    }
                }
                else {
                    throw new System.Exception("Could not find AttachedItemData scripts");
                }
            }
            //if itemID = item.id, add it to this one
            //else if itemID = -1, also add it to this one
            //else (there is another item here, let's insert this one here and find a new place for the current item)

            //Set its canvas relative position to the same position as this container (snap effect)
            count++;
            countText.SetActive(!(count < 2)); //Disable count text if there is 1 or none
            countTextValue.text = count.ToString();
            RectTransform rectTransformOfDroppedItem = itemDroppedIntoSlot.GetComponent<RectTransform>();
            rectTransformOfDroppedItem.SetParent(transform, false);
            rectTransformOfDroppedItem.anchorMin = currentSlotRectTransformAnchorMin;
            rectTransformOfDroppedItem.anchorMax = currentSlotRectTransformAnchorMax;
            rectTransformOfDroppedItem.anchoredPosition = currentSlotRectTransformAnchorPos;
            //itemDroppedIntoSlot.GetComponent<RectTransform>() = rectTransformOfDroppedItem;

        }
    }
}
