using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantKillSpell : SpellAction {
    /**
	 * Level 1: Radius: 1.5, Timeout: 2
	 * Level 2: Radius: 2, Timeout: 3.5
	 * Level 3: Radius: 2.25, Timeout: 5
	 */
    bool kill = false;
    public override void EnableSpell() {
        //Radius and timeout taken care of for us by base class
        kill = true;
        //Now set the destroy timer
        base.EnableSpell();
    }
    private void Update() {
        if (kill) {
            foreach (var collider in Physics.OverlapSphere(transform.position, attackRadius, GameManager.LAYER_Enemy)) {
                EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
                enemyController.killEnemy(); //TODO: provide extra XP if this spell was used in the same place as a lure spell
            }
        }
    }

}
