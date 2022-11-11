using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAddInventory : MonoBehaviour, IPointerDownHandler {
    //This class should be added to 2D prefabs that can be clicked on to add to inventory
    public void OnPointerDown(PointerEventData eventData) {
        //We were clicked on, now add this item to inventory
        string itemType = GetComponent<AttachedItemData>().itemType;
        switch (itemType.ToLower()) {
            case "potion":
                gameObject.SetActive(false);
                AttachedPotionData potionData = GetComponent<AttachedPotionData>();
                Potion thisPotion = potionData.toPotionClass();
                InventoryManager.Instance.pickupItem(thisPotion);
                break;
            case "rawmaterial":
                AttachedRawMaterialData rawMaterialData = GetComponent<AttachedRawMaterialData>();
                RawMaterial thisRawMaterial = rawMaterialData.toRawMaterialClass();
                InventoryManager.Instance.pickupItem(thisRawMaterial);
                break;
        }
        //Now remove this object from the game altogether
        //Destroy(eventData.pointerPress);
        Destroy(gameObject);
    }
}
