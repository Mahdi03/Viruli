using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Doors/Create New Door")]
public class MainDoor : ScriptableObject {

    [Serializable]
    public struct DoorStats {
        public DoorStats(int currentLevel, int maxHealth, int damageDealt) {
            this.currentLevel = currentLevel;
            this.maxHealth = maxHealth;
            this.damageDealt = damageDealt;
            this.doorPrefab = null;
        }
        public int currentLevel;
        public int maxHealth;
        public int damageDealt;
        public GameObject doorPrefab;
    }

    [SerializeField]
    private bool bigDoor; //Use this just as a reminder to me when I'm setting the data

    [SerializeField]
    private string parentTransformTagName;
    private Transform parentTransform; //Where to spawn the door

    //public int initialHealth; //Have small doors have lesser health than the big doors

    public List<DoorStats> doorInfo = new List<DoorStats>() {
            new DoorStats(currentLevel: 1, 100, 0),
            new DoorStats(currentLevel: 2, 100 + 100, 0),
            new DoorStats(currentLevel: 3, 100 + 100 + 200, 4)
        }; //Index of list designates level

    private void Awake() {
        
        parentTransform = GameObject.FindGameObjectWithTag(parentTransformTagName).transform;
    }
    
    


    
}
