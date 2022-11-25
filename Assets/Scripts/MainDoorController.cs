using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDoorController : MonoBehaviour {

	private int maxHealth = 200, currentHealth;
	[SerializeField]
	private HealthBarBehavior healthBarBehaviorScript;

	private void Start() {
		currentHealth = maxHealth;
		updateHealthBar();
		updateHealthBar();
	}
	public void DamageHealth(int dealsDamage) {
		currentHealth -= dealsDamage;
		updateHealthBar();
		if (currentHealth <= 0) {
			//Oops the zombies broke through a door!! game overrrr
			GameManager.Instance.GameOver();
		}
	}

	private void updateHealthBar() {
		healthBarBehaviorScript.UpdateHealthBar(currentHealth, maxHealth);
	}
}
