using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ClickAddQuickInventory : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        //Only register on left clicks, we want right clicks to be able to discard items I think?
        if (eventData.button == 0) {
            
            //The ItemInstance stores the associated inventorySlotID
            var itemWeClickedOn = eventData.pointerClick;
            ItemInstance itemInstance = itemWeClickedOn.GetComponent<ItemInstance>();
            int inventoryIndexOfClickedItem = itemInstance.attachedInventorySlotID;

            //We need to swap this object with an empty spot in the first 0-9 slots of the inventory, if that exists
            GameObject[] inventorySlots = GameObject.FindGameObjectsWithTag("inventorySlot"); //Find all GameObjects that are inventorySlots
            for (int i = 0; i < 10; i++) {
                //Loop thru only the first 10 inventory items to find whether we have an empty slot
                var inventorySlot = inventorySlots[i];
                if (inventorySlot.transform.childCount < 3) {
                    //We found an empty slot of slotID = i (inventory index = i)

                    //Now let's swap this empty position for the posiiton of our object
                    InventoryManager.Instance.swapItemsInInventory(i, inventoryIndexOfClickedItem);

                    break;
                }
            }
            //If we made it out of the for loop then if we broke the loop, we swapped, and if we couldn't then nothing happened
        }
    }

}
