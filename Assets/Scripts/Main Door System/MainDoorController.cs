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

	public (int, int) getCurrentHealthStats() {
		return (this.currentHealth, this.maxHealth);
	}
	public int getLevel() {
	return this.Level;
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


	public void GlowDoor() { doorGlow.SetActive(true); }
	public void UnglowDoor() { doorGlow.SetActive(false); }

}
