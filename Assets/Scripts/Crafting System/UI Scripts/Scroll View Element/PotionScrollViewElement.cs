using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PotionScrollViewElement : ScrollViewElementController {
    public override void OnPointerClick(PointerEventData eventData) {
        base.OnPointerClick(eventData);
        //Now load the rest of the craftable item's data from the database into the UI using the itemID
        CraftingUIController.Instance.itemID = this.itemID;
        CraftingUIController.Instance.LoadCraftableUI();
    }
}
