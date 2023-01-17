using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSpell : SpellAction {
	/**
	 * Level 1: 3 sec delay per hit
	 * Level 2: 1 sec delay per hit
	 * Level 3: 0.25 sec delay per hit
	 */
	private bool startPoisoning = false;
	private float delay = 10000f;
	public override void EnableSpell() {
		//Radius and timeout taken care of for us by base class
		//First set stats based on what level spell we are
		int level = InGameItemsDatabaseManager.Instance.getItemByID(itemID).spellLevel;
		if (level == 1) {
			delay = 2f;
		}
		else if (level == 2) {
			delay = 0.75f;
		}
		else if (level == 3) {
			delay = 0.25f;
		}
		else {
			//Debug.Log("OOPS how did we make it here?!?!?");
		}
		startPoisoning = true;
		//Set the destroy timer
		base.EnableSpell();
	}

	protected override void Update() {
		base.Update();
		if (startPoisoning) {
			foreach (var collider in Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy)) {
				EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
				enemyController.Poison(delay); //EnemyController already has a poison coroutine defined
			}
		}
	}
}
