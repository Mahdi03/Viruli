using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour {
    private static EnemySpawner instance;
    public static EnemySpawner Instance { get { return instance; } }

    [SerializeField]
    private TextMeshProUGUI roundCounterTextbox; //Double use, make it a timer or display the round

    private void Awake() {
        //Singleton initialization code
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            //Now we can initialize stuff

        }
    }

    [SerializeField]
    private Transform enemiesContainer;

    private float spawnRadius = 7f;


    // Start is called before the first frame update
    void Start() {
        stopRoundBreak(); //We can use this to kickstart the game too
    }

    private float spawnDelay;
    private int roundNumber = 0; //Start it off at 0 since the `stopRoundBreak()` will kick things off
    private int enemiesSpawned = 0;
    private int enemiesToSpawnThisRound = 0;
    private IEnumerator spawner() {
        spawnRandomEnemy();
        switch (roundNumber) {
            case 1:
            case 2:
                spawnDelay = Random.Range(4f, 6f);
                break;
            case 3:
            case 4:
                spawnDelay = Random.Range(3f, 5f);
                break;
            case 5:
            case 6:
                spawnDelay = Random.Range(2f, 4f);
                break;
            case 7:
            case 8:
                spawnDelay = Random.Range(2f, 3f);
                break;
            case 9:
            case 10:
                spawnDelay = Random.Range(1f, 2f);
                break;
            default:
                throw new System.IndexOutOfRangeException("How did we get to round #" + roundNumber);
                break;
        }

        yield return new WaitForSeconds(spawnDelay);
        Debug.Log("enemiesToSpawnThisRound: " + enemiesToSpawnThisRound);
        Debug.Log("enemiesSpawned: " + enemiesSpawned);
        if (enemiesSpawned < enemiesToSpawnThisRound) {
            StartCoroutine(spawner());
        }
    }
    private int roundDelay = 30; //30 seconds in between rounds, we can vary this later
    private int timeRemaining;
    private void startRoundBreak() {
        timeRemaining = roundDelay;
        StartCoroutine(updateTimer());
        GameManager.Instance.SaveGame(this.roundNumber); //Save game between rounds
    }
    private IEnumerator updateTimer() {
        timeRemaining--;
        string timeToPrint = "00:";
        if (timeRemaining > 9) {
            timeToPrint += timeRemaining;
        }
        else {
            timeToPrint += "0" + timeRemaining;
        }
        roundCounterTextbox.text = timeToPrint;
        yield return new WaitForSeconds(1);
        if (timeRemaining <= 0) {
            stopRoundBreak();
        }
        else {
            StartCoroutine(updateTimer());
        }
    }
    private void stopRoundBreak() {
        //Set all the correct values
        roundNumber++;
        if (!(roundNumber > 10)) {
            roundCounterTextbox.text = "Round " + roundNumber;
            enemiesToSpawnThisRound = 10 + 5 * (roundNumber);
            //enemiesToSpawnThisRound = 1 + 2 * (roundNumber);
            enemiesSpawned = 0;
            //enemiesToSpawnThisRound = roundNumber;
            Debug.Log("roundNumber: " + roundNumber);
            //Actually start the spawning again
            StartCoroutine(spawner());
        }
        else {
            SceneManager.LoadScene("WinGame", LoadSceneMode.Additive);
        }
    }
    /*
    private IEnumerator startRoundBreak() {
        //TODO: Here we need to save all the game data so that we can load in between rounds
        yield return new WaitForSeconds(roundDelay); //TODO: maybe instead of one 30 sec break, every 1 sec make a timer somewhere on the screen to show a round break

        //We are guaranteed that if round break was called, we are still in the game
        
        StartCoroutine(spawner());

    }
    */
    private void spawnRandomEnemy() {
        //Step #1: Pick an enemy from random to spawn
        //TODO: Make round 1&2 only zombies, add trolls lvl 3 and minotaurs in lvl 5
        var listOfEnemies = InGameItemsDatabaseManager.Instance.enemiesToSpawnFrom; //weighted probability in between the different enemies
        float enemyToSpawn = Random.Range(0, listOfEnemies.Count);
        Enemy chosenEnemy = InGameItemsDatabaseManager.Instance.getEnemyByID(listOfEnemies[(int)enemyToSpawn]);
        //Scale the enemy health based on what round we are to make the game harder
        int adjustedEnemyHealth = chosenEnemy.maxHealth;
        switch (roundNumber) {
            case 1:
            case 2:
            case 3:
                adjustedEnemyHealth = chosenEnemy.maxHealth * 1;
                break;
            case 4:
            case 5:
            case 6:
                adjustedEnemyHealth = (int)(chosenEnemy.maxHealth * 1.5);
                break;
            case 7:
            case 8:
            case 9:
            case 10:
                adjustedEnemyHealth = (int)(chosenEnemy.maxHealth * 2);
                break;
        }

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
        enemyController.initStats(
            enemyName: chosenEnemy.name,
            speed: chosenEnemy.speed,
            maxHealth: adjustedEnemyHealth,
            dealsDamage: chosenEnemy.dealsDamage,
            attackRadius: chosenEnemy.attackRadius,
            xpValue: chosenEnemy.xpValue,
            minItemDropCount: chosenEnemy.minItemDropCount,
            maxItemDropCount: chosenEnemy.maxItemDropCount);
        enemiesSpawned++;
        Debug.Log(chosenEnemy.name);
    }
    private int enemyKillCounter = 0;
    public void EnemyKilled() {
        enemyKillCounter++;
        Debug.Log("enemyKillCounter: " + enemyKillCounter);
        if (enemyKillCounter >= enemiesToSpawnThisRound) {
            enemyKillCounter = 0;
            //The last enemy was just killed, we can start the round break now
            startRoundBreak();
            //Reset values
            
        }
    }

}
