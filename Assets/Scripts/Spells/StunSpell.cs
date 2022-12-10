using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunSpell : SpellAction {
	bool startStunning = false;
	public override void EnableSpell() {
		//Radius and timeout taken care of for us by base class
		startStunning = true;
		//Now set the destroy timer
		base.EnableSpell();
	}
	protected override void Update() {
        base.Update();
        if (startStunning) {
            foreach (var collider in Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy)) {
                EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
                enemyController.changeMovementSpeed(0);
				enemyController.isStunned = true;
            }
        }
    }
	protected override void EndSpellEffects() {
		//We need to redirect the zombies to their nearest door again
		foreach (var collider in Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy)) {
			EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
			enemyController.changeMovementSpeed(enemyController.BaseMovementSpeed);//Restore their original speed
			enemyController.isStunned = false;
		}
		base.EndSpellEffects();
	}
}
