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

	[SerializeField]
	private GameObject doorGlow;

	private bool bigDoor;

	private GameObject twoDimensionalPrefab;

	public void initStats(bool isBigDoor, int level, float attackRange, int currentHealth, int maxHealth, int damageDealt, GameObject twoDimensionalPrefab) {
		this.bigDoor = isBigDoor;
		this.Level= level;
		this.attackRange = attackRange;
		this.maxHealth = maxHealth;
		this.currentHealth = currentHealth;
		this.damageDealt = damageDealt;
		this.twoDimensionalPrefab = twoDimensionalPrefab;

        updateHealthBar();
        updateHealthBar();
		/*
		//if (damageDealt != 0 ) {
			StartCoroutine(DoorAttack());
		//}
		*/
    }

	public (int, int) getCurrentHealthStats() {
		return (this.currentHealth, this.maxHealth);
	}
	public int getLevel() {
	return this.Level;
	}
	private bool isAlive = true;
	public void DamageHealth(int dealsDamage, Transform enemyTransform) {

		//Play a door breaking sound
		MainDoorManager.Instance.PlayRandomDoorAttackNoise();

		currentHealth -= dealsDamage;
		updateHealthBar();
		if (damageDealt != 0) {
			enemyTransform.GetComponent<EnemyController>().DamageHealth(1/*damageDealt*/);
		}
		if (currentHealth <= 0) {
			if (isAlive) {
				isAlive = false;
				//Oops the zombies broke through a door!! game overrrr
				MainDoorManager.Instance.PlayDoorBreakNoise();
				GameManager.Instance.GameOver();
			}
		}
	}

	public void Repair() {
		currentHealth = maxHealth;
		updateHealthBar();
	}

	private void updateHealthBar() {
		healthBarBehaviorScript.UpdateHealthBar(currentHealth, maxHealth);
	}


	public void GlowDoor() { doorGlow.SetActive(true); }
	public void UnglowDoor() { doorGlow.SetActive(false); }

}
