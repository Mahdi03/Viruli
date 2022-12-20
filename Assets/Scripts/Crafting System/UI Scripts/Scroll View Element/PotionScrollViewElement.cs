using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PotionScrollViewElement : ScrollViewElementController {
    public override void OnPointerClick(PointerEventData eventData) {
        if (!this.disabled) {
        	base.OnPointerClick(eventData);
        	//Now load the rest of the craftable item's data from the database into the UI using the itemID
        	CraftingUIPotionsManager.Instance.itemID = this.itemID;
        	CraftingUIPotionsManager.Instance.LoadCraftableUI();
        }
        else {
            //We need to unlock the potion first, show the user this info
            //TODO: Show user message
            
        }
    }
}