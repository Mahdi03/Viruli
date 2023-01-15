using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			dealsDamage = 3;
		}
		else if (level == 2) {
			dealsDamage = 5;
		}
		else if (level == 3) {
			dealsDamage = 15;
		}
		else {
			Debug.Log("OOPS how did we make it here?!?!?");
		}

         attackDelay = new WaitForSeconds(delay);
        //Now actually define what happens with the spell
        StartCoroutine(DealDamage());
		//Now set the destroy timer
		base.EnableSpell();
	}
	private float delay = 0.75f;
	WaitForSeconds attackDelay;
	private IEnumerator DealDamage() {
		yield return attackDelay;

		//Get all enemies in current space and for each enemy deal damage
		Collider[] colliders = Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy);

		foreach (var collider in Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy)) {
			EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
			enemyController.DamageHealth(dealsDamage);
		}
		StartCoroutine(DealDamage()); //And then repeat
	}
}
