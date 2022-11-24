using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    private float movementSpeed;
    private float currentMovementSpeed;

    private int maxHealth;
    private int currentHealth;
    private HealthBarBehavior healthBarBehaviorScript;

    private Transform target;
    private bool isAttacking = false;
    private float attackRadius;
    private int dealsDamage;


    private NavMeshAgent meshAgent;
    private EnemyAnimator enemyAnimator;
    private EnemyMotor enemyMotor;


    // Start is called before the first frame update
    void Start() {
        meshAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyMotor = GetComponent<EnemyMotor>();

        //Set current health to maxHealth possible
        currentHealth = maxHealth;
        
        //Get the health bar above the player and set its value
        healthBarBehaviorScript = transform.GetChild(0).gameObject.GetComponent<HealthBarBehavior>();
        updateHealthBar();
        updateHealthBar();
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
    }
    public void killEnemy() { //Make public for instant death potion
        //Tell DatabaseManager to drop some items for killing the enemy
        for (int i = 0; i < Random.Range(1, 4); i++) { //TODO: change this to a range of values put in through the editor (bigger enemies = bigger rewards)
            InGameItemsDatabaseManager.Instance.DropRandomItem(transform.position, Quaternion.identity);
        }
        Destroy(gameObject);

    }

    bool closeEnoughToAttack() {
        return Vector3.Magnitude(transform.position - target.position) <= meshAgent.stoppingDistance;
    }

    // Update is called once per frame
    void Update() {
        if (target) {
            enemyMotor.moveToTarget();
            if (closeEnoughToAttack() && !isAttacking) {
                isAttacking = true;
                enemyAnimator.SetTrigger("Attack");
            }
        }
        else {
            Debug.Log("Target nonexistent");
        }
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
