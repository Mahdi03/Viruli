using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInitializer : MonoBehaviour {
    public Transform enemiesObj;
    // Start is called before the first frame update
    void Start() {
        foreach (GameObject zombie in enemiesObj) { 
            EnemyController zombieController = zombie.GetComponent<EnemyController>();
            
        }
    }
}
