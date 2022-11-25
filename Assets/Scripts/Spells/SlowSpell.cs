using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowSpell : SpellAction {
	/**
	 * Level 1: 75% movement speed, Radius: ?, Timeout: const
	 * Level 2: 50% movement speed, Radius: ?, Timeout: const
	 * Level 3: 25% movement speed, Radius: ?, Timeout: const
	 */
	private bool startSlowingDown = false;
	private float slowDownFactor = 1f;
	public override void EnableSpell() {
		//Radius and timeout taken care of for us by base class
		//First set stats based on what level spell we are
		int level = InGameItemsDatabaseManager.Instance.getItemByID(itemID).spellLevel;
		if (level == 1) {
			slowDownFactor = 0.75f;
		}
		else if (level == 2) {
			slowDownFactor = 0.5f;
		}
		else if (level == 3) {
			slowDownFactor = 0.25f;
		}
		else {
			Debug.Log("OOPS how did we make it here?!?!?");
		}

		startSlowingDown = true;
		//Now set the destroy timer
		base.EnableSpell();
	}
	private void Update() {
		if (startSlowingDown) {
			foreach (var collider in Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy)) {
				EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
				enemyController.changeMovementSpeed(enemyController.BaseMovementSpeed * slowDownFactor); //Reduce speed
			}
		}
	}
	protected override void EndSpellEffects() {
		//We need to redirect the zombies to their nearest door again
		foreach (var collider in Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy)) {
			EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
			enemyController.changeMovementSpeed(enemyController.BaseMovementSpeed); //Reset speed
		}
		base.EndSpellEffects();
	}
}
