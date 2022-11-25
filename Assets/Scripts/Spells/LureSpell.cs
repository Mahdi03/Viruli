using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureSpell : SpellAction {
    /**
	 * Level 1: Radius: 4, Timeout: 10
	 * Level 2: Radius: 7, Timeout: 12.5
	 * Level 3: Radius: 10, Timeout: 16
	 */
    bool startLuring = false;
    public override void EnableSpell() {
        //Radius and timeout taken care of for us by base class
        startLuring = true;
        //Now set the destroy timer
        base.EnableSpell();
    }
    private void Update() {
        if (startLuring) {
            foreach (var collider in Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy)) {
                EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
                enemyController.SetTarget(transform);//Redirect them to this spell's position
            }
        }
    }
    protected override void EndSpellEffects() {
        //We need to redirect the zombies to their nearest door again
        foreach (var collider in Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy)) {
            EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
            enemyController.SetTarget(enemyController.findNearestDoor()); //Redirect them back to their door
        }
        base.EndSpellEffects();
    }

}
