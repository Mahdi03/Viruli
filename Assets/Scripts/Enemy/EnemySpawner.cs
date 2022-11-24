using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    private Transform enemiesContainer;

    private float spawnRadius = 7f;


    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < 10; i++) {
            spawnRandomEnemy();
        }
    }
    private void spawnRandomEnemy() {
        //Step #1: Pick an enemy from random to spawn
        var listOfEnemies = InGameItemsDatabaseManager.Instance.enemies;
        float enemyToSpawn = Random.Range(0, listOfEnemies.Count);
        //Step #2: Pick a spawn point (this is a list of approximate centers for each spawn room)
        List<Vector3> spawnRooms = new List<Vector3>() {
        new Vector3(42, 0, 0),
        new Vector3(4, 0, 66),
        new Vector3(-42, 0, 4),
        new Vector3(8, 0, -70)
        };
        float locationToSpawn = Random.Range(0, spawnRooms.Count);
        Vector3 spawnLocation = Random.insideUnitSphere * spawnRadius + spawnRooms[(int)locationToSpawn];
        //Step #3: Spawn at location inside hierarchy parent
        var newEnemy = Instantiate(listOfEnemies[(int)enemyToSpawn].enemyPrefab, spawnLocation, Quaternion.identity, enemiesContainer);
        //Step #4: Ready the enemy
        var enemyController = newEnemy.GetComponent<EnemyController>();
        //TODO: Set target to the nearest door
        enemyController.SetTarget(enemyController.findNearestDoor());
    }
    //TODO: Set up a coroutine for rounds every 4 mins, and have each round fire a coroutine that spawns randomly
}
