using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemInstance))] //Make sure that we have item instance before we proceed
public class OnHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
    private Tooltip tooltipScript;
    private string itemName;
    private void Start() {
        GameObject tooltip = GameManager.Instance.GetTooltip();
        tooltipScript = tooltip.GetComponent<Tooltip>();
        if (tooltipScript == null) {
            throw new System.NullReferenceException("Tooltip script not attached to tooltip object");
        }

        //This script will be attached to a gameobject that is guaranteed to have an ItemInstance class with the itemID, use it
        ItemInstance itemInstance = GetComponent<ItemInstance>();
        IItem item = InGameItemsDatabaseManager.Instance.getItemByID(itemInstance.itemID);
        this.itemName = item.itemName;
    }

    /*For 2D objects*/
    public void OnPointerEnter(PointerEventData eventData) {
        tooltipScript.ShowTooltip(itemName);
    }

    public void OnPointerExit(PointerEventData eventData) {
        tooltipScript.HideTooltip();
    }
    public void OnPointerDown(PointerEventData eventData) {
        tooltipScript.HideTooltip();
    }

    /*For 3D objects*/
    private void OnMouseEnter() {
        tooltipScript.ShowTooltip(itemName);
    }
    private void OnMouseExit() {
        tooltipScript.HideTooltip();
    }
    private void OnMouseDown() {
        tooltipScript.HideTooltip();
    }

}
