using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Raw Material")]
public class RawMaterial : Item {
	public bool droppedByEnemy;
    public override string itemType { get { return this.GetType().Name; } }

    public override void showOnSceneRing() {
		base.showOnSceneRing();
		//Show a small signal on the scene that we can drop our 3-D object there
	}

}
