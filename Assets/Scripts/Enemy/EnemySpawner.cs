using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

	[SerializeField]
	private Transform enemiesContainer;

	private float spawnRadius = 7f;


	// Start is called before the first frame update
	void Start() {
		StartCoroutine(spawner());
	}

	private float spawnDelay;
	private int i = 0;
	IEnumerator spawner() {
		spawnRandomEnemy();
		spawnDelay = Random.Range(3f, 6f);
		yield return new WaitForSeconds(spawnDelay);
		if (i < 10) { //TODO: make this variable so that we can have rounds of increasingly difficult numbers
			StartCoroutine(spawner());
		}
	}
	private void spawnRandomEnemy() {
		//Step #1: Pick an enemy from random to spawn
		var listOfEnemies = InGameItemsDatabaseManager.Instance.enemies;
		float enemyToSpawn = Random.Range(0, listOfEnemies.Count);
		Enemy chosenEnemy = listOfEnemies[(int)enemyToSpawn];
		//Step #2: Pick a spawn point (this is a list of approximate centers for each spawn room)
		List<Vector3> spawnRooms = new List<Vector3>() {
		new Vector3(42, 0, 0), //South Spawn room
		new Vector3(4, 0, 66), //East spawn room
		new Vector3(-42, 0, 4), //North spawn room
		new Vector3(8, 0, -70) //West spawn room
		};
		float locationToSpawn = Random.Range(0, spawnRooms.Count);
		Vector3 spawnLocation = Random.insideUnitSphere * spawnRadius + spawnRooms[(int)locationToSpawn];
		//Step #3: Spawn at location inside hierarchy parent
		var newEnemy = Instantiate(chosenEnemy.enemyPrefab, spawnLocation, Quaternion.identity, enemiesContainer);
		//Step #4: Ready the enemy - pass in data from ScriptableObject to instance in scene
		var enemyController = newEnemy.GetComponent<EnemyController>();
		enemyController.initStats(chosenEnemy.speed, chosenEnemy.maxHealth, chosenEnemy.dealsDamage, chosenEnemy.xpValue, chosenEnemy.minItemDropCount, chosenEnemy.maxItemDropCount);
		i++;
	}
	//TODO: Set up a coroutine for rounds every 4 mins, and have each round fire a coroutine that spawns randomly
}
