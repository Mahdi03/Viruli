using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorScrollViewElement : ScrollViewElementController {
    public override void OnPointerClick(PointerEventData eventData) {
        base.OnPointerClick(eventData);
        
        //Highlight which door this is in the layout
        MainDoorManager.Instance.GlowDoorByID(this.itemID);

        //Show data on doors
        CraftingUIDoorsManager.Instance.doorID = this.itemID; //Set a door ID
        CraftingUIDoorsManager.Instance.LoadDoorUI();

    }
}
