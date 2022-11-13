using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemInstance))] //Make sure that we have item instance before we proceed
public class ClickAddInventory : MonoBehaviour, IPointerDownHandler {
    //This class should be added to 2D prefabs that can be clicked on to add to inventory
    public void OnPointerDown(PointerEventData eventData) {
        //We were clicked on, now add this item to inventory
        int itemID = GetComponent<ItemInstance>().itemID;
        InventoryManager.Instance.pickupItem(itemID);
        //Now remove this object from the game altogether
        Destroy(gameObject);
    }
}
