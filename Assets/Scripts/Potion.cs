using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Potion : Item {
	Potion() {
		this.itemType = "Potion";
	}
	public override void showOnSceneRing() {
		base.showOnSceneRing();

	}
	public override void OnEndDrag(PointerEventData eventData) {
		base.OnEndDrag(eventData);
		//For the potion we want it to create a permanent colored ring that is causing damage on the scene at that point
	}
}
