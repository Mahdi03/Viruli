using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainDoorController : MonoBehaviour {

    public int Level { get; private set; }
	private float attackRange = 4f;
	private int damageDealt;

    private int maxHealth = 200, currentHealth;
	[SerializeField]
	private HealthBarBehavior healthBarBehaviorScript;

	private bool bigDoor;

	public void initStats(bool bigDoor, int level, float attackRange, int maxHealth, int damageDealt) {
		this.bigDoor = bigDoor;
		this.Level= level;
		this.attackRange = attackRange;
		this.maxHealth = maxHealth;
		currentHealth = maxHealth;
		this.damageDealt = damageDealt;
        updateHealthBar();
        updateHealthBar();
		/*
		//if (damageDealt != 0 ) {
			StartCoroutine(DoorAttack());
		//}
		*/
    }

	public void DamageHealth(int dealsDamage, Transform enemyTransform) {
		currentHealth -= dealsDamage;
		updateHealthBar();
		if (damageDealt != 0) {
			enemyTransform.GetComponent<EnemyController>().DamageHealth(1/*damageDealt*/);
		}
		if (currentHealth <= 0) {
			//Oops the zombies broke through a door!! game overrrr
			GameManager.Instance.GameOver();
		}
	}

	public void Repair() {
		currentHealth = maxHealth;
		updateHealthBar();
	}

	private void updateHealthBar() {
		healthBarBehaviorScript.UpdateHealthBar(currentHealth, maxHealth);
	}
	/*
    //TODO: set up coroutine that damages surrounding enemies based on current damage value
    private void OnDrawGizmos() {
		Gizmos.color = Color.magenta;
		Vector3 sizeOfDoor = GetComponent<Collider>().bounds.size; //Gets 3D size of the door
		if (bigDoor) {
            Gizmos.DrawWireCube(transform.position, sizeOfDoor);
        }
		else {
            Gizmos.DrawWireCube(transform.position, sizeOfDoor + transform.forward * attackRange);
        }
    }

	IEnumerator DoorAttack() {
		damageEnemyInViscinity();
		yield return new WaitForSeconds(0.75f);
		StartCoroutine(DoorAttack());
	}

	private void damageEnemyInViscinity() {
        Vector3 sizeOfDoor = GetComponent<Collider>().bounds.size; //Gets 3D size of the door
		Collider[] colliders;
		if (bigDoor) {
			colliders = Physics.OverlapBox(transform.position, sizeOfDoor / 2, transform.rotation, GameManager.LAYER_Enemy);
        }
		else {
			colliders = Physics.OverlapBox(transform.position, sizeOfDoor / 2 + transform.forward * attackRange / 2, transform.rotation, GameManager.LAYER_Enemy);
		}
		foreach (var collider in colliders) {
			EnemyController enemyController = collider.transform.GetComponent<EnemyController>();
			enemyController.DamageHealth(damageDealt);
		}
	}
*/
}
