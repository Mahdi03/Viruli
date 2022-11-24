using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemInstance))]
public class AttackSpell : SpellAction {
	/**
	 * Level 1: -5 Health, Radius: 2, Timeout: 7
	 * Level 2: -10 Health, Radius: 2.5, Timeout: 12
	 * Level 3: -20 Health, Radius: 3, Timeout: 15
	 */

	private int dealsDamage;
	public override void EnableSpell() {
		//First set stats based on what level spell we are
		int level = InGameItemsDatabaseManager.Instance.getItemByID(itemID).spellLevel;
		if (level == 1) {
			dealsDamage = 5;
			attackRadius = 2;
			timeout = 7;
		}
		else if (level == 2) {
			dealsDamage = 10;
			attackRadius = 2.5f;
			timeout = 12;
		}
		else if (level == 3) {
			dealsDamage = 20;
			attackRadius = 3f;
			timeout = 15;
		}
		else {
			Debug.Log("OOPS how did we make it here?!?!?");
		}

        attackRadius *= 1.5f; //The starting ring for this one has radius 1.5 in Unity particle editor

        //Now actually define what happens with the spell
        StartCoroutine(DealDamage());
		//Now set the destroy timer
		base.EnableSpell();
	}
	private float delay = 0.75f;
	private IEnumerator DealDamage() {
		yield return new WaitForSeconds(delay);

		//Get all enemies in current space and for each enemy deal damage
		Collider[] colliders = Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy);

        foreach (var collider in Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy)) {
			EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
			enemyController.DamageHealth(dealsDamage);
		}
		StartCoroutine(DealDamage()); //And then repeat
	}
}
