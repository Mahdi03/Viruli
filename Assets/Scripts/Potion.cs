using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Potion")]
public class Potion : Item {
	Potion() {
		this.itemType = "Potion";
	}
	public override void showOnSceneRing() {
		base.showOnSceneRing();

	}
	public int effectRadius;
	public float timeout = 5f;
	public List<Item> recipe;
}
