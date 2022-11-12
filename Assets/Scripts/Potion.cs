using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Potion")]
public class Potion : Item {
    public GameObject effectRingPrefab;
    public int effectRadius;
    public float timeout = 5f;
    public List<(Item, int)> recipe;

    public override string itemType { get { return this.GetType().Name; } }
    public override void showOnSceneRing() {
		base.showOnSceneRing();
	}
}
