using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ItemInstance))] //Make sure that we have item instance before we proceed
public class ClickAddInventory : MonoBehaviour, IPointerDownHandler {
    //This class should be added to 2D prefabs that can be clicked on to add to inventory
    private Canvas canvas;
    private int mouseRadius = 75;

    private void Awake() {
        canvas = GameManager.Instance.mainCanvas;
    }

    public void OnPointerDown(PointerEventData eventData) {
        //TODO: If there are a lot of items in the click radius then click them all, if not then just click this one
        GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();

        //Let's simulate a circle collider
        for (int i = 0; i < 100; i++) {
            PointerEventData newPointerEventData = new PointerEventData(null);
            newPointerEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + Random.insideUnitCircle * mouseRadius;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(newPointerEventData, results);
            if (results.Count > 3) {
                foreach (RaycastResult result in results) {
                    //Debug.Log(result);
                    ClickAddInventory clickableItem = result.gameObject.transform.parent.GetComponent<ClickAddInventory>();
                    if (clickableItem != null) {
                        //Then let's pick it up
                        clickableItem.PickUpItem();
                    }
                    else {
                        clickableItem = result.gameObject.GetComponent<ClickAddInventory>();
                        if (clickableItem != null) {
                            //Then let's pick it up
                            clickableItem.PickUpItem();
                        }
                    }
                }
            }
        }

        PickUpItem(); //Remember that we were still clicked
    }

    /*For 3D Objects*/
    private void OnMouseDown() {
        PickUpItem();
    }

    private void CheckOtherItems(PointerEventData eventData) {

    }


    public void PickUpItem() {
        //We were clicked on, now add this item to inventory
        int itemID = GetComponent<ItemInstance>().itemID;
        if (InventoryManager.Instance.pickupItem(itemID)) {
            //Play the pickup item noise
            //InventoryManager.Instance.
            //Now remove this object from the game altogether
            Destroy(gameObject);
        }
        //If we made it here then there was no more room in the inventory so we cannot pick it up
    }
}