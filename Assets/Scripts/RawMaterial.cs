using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Raw Material")]
public class RawMaterial : Item {
	public bool droppedByEnemy;
	RawMaterial() {
		this.itemType = "rawMaterial";
	}
	public override void showOnSceneRing() {
		base.showOnSceneRing();
		//Show a small signal on the scene that we can drop our 3-D object there
	}
}
