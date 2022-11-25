using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour {

	private Animator animator;
	private NavMeshAgent meshAgent;
	private bool moving = true;

	// Start is called before the first frame update
	void Start() {
		animator = GetComponent<Animator>();
		meshAgent = GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame
	void Update() {
		if (moving) {
			//Normalize the velocity so we have a float between 0 and 1 for our animator
			animator.SetFloat("Speed", meshAgent.velocity.magnitude / meshAgent.speed);
		}
	}

	public void SetTrigger(string name) {
		animator.SetTrigger(name);
	}
}
