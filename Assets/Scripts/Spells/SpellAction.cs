using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface ISpellAction {
	public virtual void EnableSpell() { }
}

[RequireComponent(typeof(ItemInstance))]
public class SpellAction : MonoBehaviour, ISpellAction {

	protected int itemID;

	protected float attackRadius;
	protected float timeout;

	/*Use gizmos to visualize the Physics.OverlapSphere in the editor to see if it matches with the ring prefab - won't be called in the actual build*/
	protected virtual void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}

	protected virtual void Start() {
		itemID = GetComponent<ItemInstance>().itemID;
		IItem potion = InGameItemsDatabaseManager.Instance.getItemByID(itemID);
		attackRadius = potion.EffectRadius * 1.5f;
		timeout = potion.EffectTimeout;
	}
	public virtual void EnableSpell() {
		StartCoroutine(destroySpell());
	}
	protected virtual void EndSpellEffects() { }
	protected IEnumerator destroySpell() {
		yield return new WaitForSeconds(timeout);
		EndSpellEffects();
		Destroy(gameObject);
	}
}
