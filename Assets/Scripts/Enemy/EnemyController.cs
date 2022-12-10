using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class EnemyController : MonoBehaviour {

    private string enemyName;

    public float BaseMovementSpeed { get; private set; }
    private float movementSpeed;

    private int maxHealth;
    private int currentHealth;
    private HealthBarBehavior healthBarBehaviorScript;

    private Transform target;
    private bool isAttacking = false;
    private float attackRadius = 1f;
    //TODO: Add public attack speed controls for spells to manipulate
    private int dealsDamage;

    private int xpValue;
    private int minItemDropCount, maxItemDropCount;


    private NavMeshAgent meshAgent;
    private EnemyAnimator enemyAnimator;
    private EnemyMotor enemyMotor;

    public bool isStunned { get; set; } = false;

    public void initStats(string enemyName, float speed, int maxHealth, int dealsDamage, float attackRadius, int xpValue, int minItemDropCount, int maxItemDropCount) {
        this.enemyName = enemyName;
        this.BaseMovementSpeed = speed;
        this.movementSpeed = speed;
        this.maxHealth = maxHealth;
        this.dealsDamage = dealsDamage;
        this.attackRadius = attackRadius;
        this.xpValue = xpValue;
        this.minItemDropCount = minItemDropCount;
        this.maxItemDropCount = maxItemDropCount;
    }

    private AudioSource enemyHurtNoise, enemyKilledNoise;

    // Start is called before the first frame update
    void Start() {
        meshAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyMotor = GetComponent<EnemyMotor>();

        var attachedNoises = GetComponents<AudioSource>();
        enemyHurtNoise = attachedNoises[1]; //The second attached noise is the noise to play when this enemy is hurt
        enemyKilledNoise = attachedNoises[2]; //The third noise attached would be when the enemy is killed

        //Set current health to maxHealth possible
        currentHealth = maxHealth;

        //Get the health bar above the player and set its value
        healthBarBehaviorScript = transform.GetChild(0).gameObject.GetComponent<HealthBarBehavior>();
        updateHealthBar();
        updateHealthBar();

        changeMovementSpeed(movementSpeed);
        SetTarget(findNearestDoor());
    }

    private void updateHealthBar() {
        healthBarBehaviorScript.UpdateHealthBar(currentHealth, maxHealth);
    }
    public void DamageHealth(int damage) {
        currentHealth -= damage;
        updateHealthBar();
        if (currentHealth <= 0) {
            //Welp we ded, destroy bye bye
            killEnemy();
        }
        else {
            enemyHurtNoise.Play();
        }
    }

    //Poison code for Poison Spell
    private bool alreadyPoisoned = false;
    public void Poison(float delay) {
        if (alreadyPoisoned) {
            return; //This enemy is already running in the poison coroutine
        }
        StartCoroutine(recurringPoison(delay, 1));
    }
    IEnumerator recurringPoison(float delay, int amountToDamage) {
        this.DamageHealth(amountToDamage);
        yield return new WaitForSeconds(delay);
        StartCoroutine(recurringPoison(delay, 2 * amountToDamage + 1));
    }
    private bool isAlive = true;

    public void killEnemy() { //Make public for instant death potion
        if (isAlive) {
            enemyKilledNoise.outputAudioMixerGroup.audioMixer.GetFloat("EnemyInjuredVolume", out float volume);
            volume = Mathf.Pow(10f, volume / 20); //Convert from Log_10??
            //TODO: Learn how to pause/play this clipatpoint too (the audiosource that is created is returned by the call)
            AudioSource.PlayClipAtPoint(enemyKilledNoise.clip, Camera.main.transform.position, volume); //Use play clip at point in case this object is destroyed before it can finish playing the sound
            //enemyKilledNoise.PlayOneShot(enemyKilledNoise.clip); 
            isAlive = false;
            //Notify the Enemy Spawner that another enemy has been killed (it is keeping track to know when to start the next round)
            EnemySpawner.Instance.EnemyKilled();
            //Tell DatabaseManager to drop some items for killing the enemy
            for (int i = 0; i < (int)Random.Range(minItemDropCount, maxItemDropCount); i++) {
                InGameItemsDatabaseManager.Instance.DropRandomItem(transform.position, Quaternion.identity, this.enemyName);
            }
            XPSystem.Instance.increaseXP(xpValue); //Add XP on enemy death
            Destroy(gameObject);
        }

    }

    bool closeEnoughToAttack() {
        return Vector3.Magnitude(transform.position - target.position) <= meshAgent.stoppingDistance;
    }
    private int doorMask = GameManager.LAYER_MainDoor;
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + transform.forward * attackRadius, attackRadius);
    }
    public void Attack() { //Called from the animator in the middle of an attack animation
        isAttacking = false;
        //var yeet = Physics.OverlapSphere(transform.position + transform.forward * attackRadius, attackRadius, doorMask);

        foreach (var collider in Physics.OverlapSphere(transform.position + transform.forward * attackRadius, attackRadius, doorMask)) {
            MainDoorController doorController = collider.transform.GetComponentInChildren<MainDoorController>();
            doorController.DamageHealth(dealsDamage, transform);
        }
    }
    // Update is called once per frame
    void Update() {
        if (target) {
            if (closeEnoughToAttack() && !isAttacking && !isStunned) {
                isAttacking = true;
                if (gameObject.name.ToLower().Contains("troll")) {
                    //We are the troll, so we have 4 attacks to choose from
                    int attack = Random.Range(0, 4);
                    enemyAnimator.SetInteger("AttackIndex", attack);
                }
                enemyAnimator.SetTrigger("Attack");
            }
        }
        else {
            Debug.Log("Target nonexistent");
            SetTarget(findNearestDoor());
        }

        //Add logic to pause sounds and play sounds
        var allAudioSources = GetComponents<AudioSource>();
        if (GameManager.Instance.IS_GAME_PAUSED) {
            //Since the game is paused, we need to pause the music
            foreach (var audioSource in allAudioSources) {
                audioSource.Pause();
            }
        }
        else {
            foreach (var audioSource in allAudioSources) {
                audioSource.UnPause();
            }
        }
    }
    public void changeMovementSpeed(float speed) {
        if (!enemyMotor) {
            enemyMotor = GetComponent<EnemyMotor>();
        }
        enemyMotor.changeMovementSpeed(speed);
    }
    public void SetTarget(Transform target) { //Make public for 
        if (!enemyMotor) {
            enemyMotor = GetComponent<EnemyMotor>();
        }
        enemyMotor.SetTarget(target);
        this.target = target;
    }
    
    public Transform findNearestDoor() {
        var doors = GameObject.FindGameObjectsWithTag("MainDoor");
        int minDistanceDoorIndex = 0;
        for (int doorIndex = 0; doorIndex < doors.Length; doorIndex++) {
            var door = doors[doorIndex];
            if (Vector3.Distance(transform.position, doors[doorIndex].transform.position) < Vector3.Distance(transform.position, doors[minDistanceDoorIndex].transform.position)) {
                //We found a new minimum, let's update it
                minDistanceDoorIndex = doorIndex;
            }
        }
        //At the end of this loop we should have our closest door
        return doors[minDistanceDoorIndex].transform;
    }
}
