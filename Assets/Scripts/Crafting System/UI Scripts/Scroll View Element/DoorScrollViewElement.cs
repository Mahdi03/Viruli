using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorScrollViewElement : ScrollViewElementController {
    public override void OnPointerClick(PointerEventData eventData) {
        base.OnPointerClick(eventData);
        //TODO: Show data on doors

        //TODO: Highlight which door this is in the layout
        MainDoorManager.Instance.GlowDoorByID(this.itemID);
    }
}
