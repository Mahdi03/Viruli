using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour {
    private static EnemySpawner instance;
    public static EnemySpawner Instance => instance;

    [SerializeField]
    private TextMeshProUGUI enemyKillCounterTextbox;

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
    private GameObject craftingOverlay;

    [SerializeField]
    private Transform enemiesContainer;

    private float spawnRadius = 7f;
    private const int finalRound = 10;

    // Start is called before the first frame update
    void Start() {

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
                spawnDelay = Random.Range(2f, 4f);
                break;
            case 5:
            case 6:
                spawnDelay = Random.Range(1f, 3f);
                break;
            case 7:
            case 8:
                spawnDelay = Random.Range(1f, 2f);
                break;
            case 9:
            case 10:
                spawnDelay = Random.Range(0f, 1f);
                break;
            default:
                throw new System.IndexOutOfRangeException("How did we get to round #" + roundNumber);
                break;
        }

        yield return new WaitForSeconds(spawnDelay);
        //Debug.Log("enemiesToSpawnThisRound: " + enemiesToSpawnThisRound);
        //Debug.Log("enemiesSpawned: " + enemiesSpawned);
        if (enemiesSpawned < enemiesToSpawnThisRound) {
            _ = StartCoroutine(spawner());
        }
    }
    private int roundDelay = 30; //30 seconds in between rounds, we can vary this later
    private int timeRemaining;
    private void Update() {
        if (currentlyInRoundBreak && Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.IS_GAME_PAUSED) {
            //Let's cancel the round break and move to the next round
            StopCoroutine("updateTimer");
            stopRoundBreak();
        }
    }
    private void startRoundBreak() {
        currentlyInRoundBreak = true;
        timeRemaining = roundDelay;
        _ = StartCoroutine("updateTimer");
        GameManager.Instance.SaveGame(this.roundNumber); //Save game between rounds
        if (this.roundNumber == 1) {
            MessageSystem.Instance.PostMessage("You can press \"Space\" to skip round breaks", alert: true);
        }

    }

    WaitForSeconds oneSecondDelay = new WaitForSeconds(1);

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
        yield return oneSecondDelay;
        if (timeRemaining <= 0) {
            stopRoundBreak();
        }
        else {
            _ = StartCoroutine("updateTimer");
        }
    }

    public bool currentlyInRoundBreak { get; private set; } = false;

    private void stopRoundBreak() {
        currentlyInRoundBreak = false;
        //Set all the correct values
        roundNumber++;
        if (!(roundNumber > finalRound)) {
            roundCounterTextbox.text = "Round " + roundNumber;
            //For the first 4 rounds use a parabolic growth, but then slow it down so we don't exceed like 200 enemies a round
            if (roundNumber < 5) {
                //Use parabolic growth function
                //(10/3.2)x^2 + 7
                enemiesToSpawnThisRound = (int)(7 + 10f / 3.2f * Mathf.Pow(roundNumber, 2));
            }
            else {
                //15 * sqrt(x-4)+57 (start off at same position as last one, just grow slower)
                //enemiesToSpawnThisRound = (int)(15 * Mathf.Pow(roundNumber, 1f/2f) + 57);
                //1/7 e^(x-2) + 85
                enemiesToSpawnThisRound = (int)(85 + Mathf.Exp(roundNumber - 2) / 7);
            }
            enemyKillCounterTextbox.text = "Enemies Killed: 0/" + enemiesToSpawnThisRound;
            //enemiesToSpawnThisRound = 10 + 5 * (roundNumber); //make exponential enemy spawner
            //enemiesToSpawnThisRound = 1 + 2 * (roundNumber);
            enemiesSpawned = 0;
            //enemiesToSpawnThisRound = roundNumber;
            //Debug.Log("roundNumber: " + roundNumber);
            //Actually start the spawning again
            _ = StartCoroutine(spawner());
            if (craftingOverlay.activeSelf) {
                //The crafting menu is still open tho, let's pause time, closing the crafting menu will take care of the rest
                Time.timeScale = 0;
            }
        }
        else {
            SceneManager.LoadScene("WinGame", LoadSceneMode.Additive);
        }
    }
    /*
    private IEnumerator startRoundBreak() {
        //Here we need to save all the game data so that we can load in between rounds
        yield return new WaitForSeconds(roundDelay); //maybe instead of one 30 sec break, every 1 sec make a timer somewhere on the screen to show a round break

        //We are guaranteed that if round break was called, we are still in the game
        
        StartCoroutine(spawner());

    }
    */
    private void spawnRandomEnemy() {
        //Step #1: Pick an enemy from random to spawn
        List<int> listOfEnemies = InGameItemsDatabaseManager.Instance.enemiesToSpawnFrom; //weighted probability in between the different enemies
        float enemyToSpawn = Random.Range(0, listOfEnemies.Count);
        Enemy chosenEnemy = InGameItemsDatabaseManager.Instance.getEnemyByID(listOfEnemies[(int)enemyToSpawn]);
        //Scale the enemy health based on what round we are to make the game harder
        int adjustedEnemyHealth = chosenEnemy.maxHealth;
        switch (roundNumber) {
            case 1:
            case 2:
                adjustedEnemyHealth = chosenEnemy.maxHealth * 1;
                break;
            case 3:
            case 4:
                adjustedEnemyHealth = (int)(chosenEnemy.maxHealth * 1.5);
                break;
            case 5:
            case 6:
            case 7:
                adjustedEnemyHealth = chosenEnemy.maxHealth * 2;
                break;
            case 8:
            case 9:
            case 10:
                adjustedEnemyHealth = chosenEnemy.maxHealth * 3;
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
        if (chosenEnemy.name.Contains("Zombie")) {
            //Try spawning them in hordes
            int numZombiesToSpawn = Random.Range(1, 3);
            for (int i = 0; i < numZombiesToSpawn; i++) {
                if (enemiesSpawned < enemiesToSpawnThisRound) {
                    //Step #3: Spawn at location inside hierarchy parent
                    GameObject newEnemy = Instantiate(chosenEnemy.enemyPrefab, spawnLocation, Quaternion.identity, enemiesContainer);
                    //Step #4: Ready the enemy - pass in data from ScriptableObject to instance in scene
                    EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
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
                    //Debug.Log(chosenEnemy.name);
                }
            }
        }
        else {
            if (enemiesSpawned < enemiesToSpawnThisRound) {
                //Step #3: Spawn at location inside hierarchy parent
                GameObject newEnemy = Instantiate(chosenEnemy.enemyPrefab, spawnLocation, Quaternion.identity, enemiesContainer);
                //Step #4: Ready the enemy - pass in data from ScriptableObject to instance in scene
                EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
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
                //Debug.Log(chosenEnemy.name);
            }
        }

    }
    private int enemyKillCounter = 0;
    public void EnemyKilled(string enemyName) {
        enemyKillCounter++;
        enemyKillCounterTextbox.text = "Enemies Killed: " + enemyKillCounter + "/" + enemiesToSpawnThisRound;

        enemyName = enemyName.ToLower();
        if (enemyName.Contains("zombie")) {
            GameManager.Instance.persistentGameStatsData.zombiesKilled++;
        }
        else if (enemyName.Contains("troll")) {
            GameManager.Instance.persistentGameStatsData.trollsKilled++;
        }
        else if (enemyName.Contains("minotaur")) {
            GameManager.Instance.persistentGameStatsData.minotaursKilled++;
        }

        //Debug.Log("enemyKillCounter: " + enemyKillCounter);
        //MessageSystem.Instance.PostMessage("Enemies Killed: " + enemyKillCounter + "/" + enemiesToSpawnThisRound, muted: true);
        if (enemyKillCounter >= enemiesToSpawnThisRound) {
            enemyKillCounter = 0;

            if (finalRound == roundNumber) {
                //We are currently in the n-1 round
                stopRoundBreak(); //Stop round break has the logic to end the game right there and then

            }
            else {
                //The last enemy was just killed, we can start the round break now
                startRoundBreak();
            }
        }
    }

    public void LoadRound(int roundNumber) {
        this.roundNumber = roundNumber;
        stopRoundBreak(); //We can use this to kickstart the game too
    }
}