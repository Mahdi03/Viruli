using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PotionScrollViewElement : ScrollViewElementController {

    public override void OnPointerClick(PointerEventData eventData) {
        
        IItem potion = InGameItemsDatabaseManager.Instance.getItemByID(this.itemID);
        this.disabled = XPSystem.Instance.Level < potion.spellXPLevelUpgrade;


        if (!this.disabled) {
            base.OnPointerClick(eventData);
            //Now load the rest of the craftable item's data from the database into the UI using the itemID
            CraftingUIPotionsManager.Instance.itemID = this.itemID;
        	CraftingUIPotionsManager.Instance.LoadCraftableUI();
        }
        else {
            //We need to unlock the potion first, show the user this info
            //Show user message
            

            MessageSystem.Instance.PostMessage("This potion has not been unlocked yet!! Reach XP Level " + potion.spellXPLevelUpgrade + " to unlock " + potion.itemName + ".", alert: true);
            //Debug.Log("This potion has not yet been unlocked");
        }
    }
}