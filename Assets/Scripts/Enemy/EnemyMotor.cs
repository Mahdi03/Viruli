using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor : MonoBehaviour {

    private NavMeshAgent meshAgent;

    private Transform target;
    public float rotationSpeed;


    // Start is called before the first frame update
    void Start() {
        meshAgent = GetComponent<NavMeshAgent>();
    }
    public void moveToTarget() {
        if (target) {
            meshAgent.SetDestination(target.position);
        }
        else {
            Debug.Log("Target isn't set");
        }
    }
    public void changeMovementSpeed(float speed) {
        meshAgent.speed = speed;
    }
    public void nullTarget() {
        target = null;
    }
    public void SetTarget(Transform t) {
        target = t;
        moveToTarget();
    }

    public void Move(Vector3 pos) {
        nullTarget();
        meshAgent.isStopped = false;
        meshAgent.SetDestination(pos);
    }
    public void Stop() {
        meshAgent.isStopped = true;
        meshAgent.velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update() {
        //Make a smooth rotation
        if (target) {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            Quaternion linearInterpolationRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);
            transform.rotation = linearInterpolationRotation;
        }
    }
}
