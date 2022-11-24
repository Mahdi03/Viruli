using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Potion")]
public class Potion : Item {

	public override int spellLevel {
		get {
			if (itemName.Contains("(III)")) {
				return 3;
			}
			else if (itemName.Contains("(II)")) {
				return 2;
			}
			else if (itemName.Contains("(I)")) {
				return 1;
			}
			else { return -1; }
		}
	}

	[SerializeField]
	private float effectRadius;
	public override float EffectRadius { get => effectRadius; }

	[SerializeField]
	private float timeout = 5f;
	public override float EffectTimeout { get => timeout; }

	/* We want to make publicly available
	 * List<(int, int)>
	 * (itemID, countRequired)
	 * but Unity editor doesn't support tuples and I want to be able to pick the actual IItem so instead I'll
	 * grab Items and then put them into an array of IItem and then get their ID's
	 */

	public override string itemType { get { return this.GetType().Name; } }
}
