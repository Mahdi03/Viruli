using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }


    public GameObject tooltipObjInScene;

    private void Awake() {
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            //Now we can instantiate stuff if needed
        }
    }
    public GameObject GetTooltip() { return tooltipObjInScene; }
}