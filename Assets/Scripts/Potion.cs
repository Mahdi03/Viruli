using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Potion")]
public class Potion : Item {
    public GameObject effectRingPrefab;
    public int effectRadius;
    public float timeout = 5f;

    /* We want to make publicly available
     * List<(int, int)>
     * (itemID, countRequired)
     * but Unity editor doesn't support tuples and I want to be able to pick the actual IItem so instead I'll
     * grab Items and then put them into an array of IItem and then get their ID's
     */

    public override string itemType { get { return this.GetType().Name; } }
    public override void showOnSceneRing() {
		base.showOnSceneRing();
	}
}
