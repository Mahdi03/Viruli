using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    /// <summary>
    /// Publicly defined method to remove all children of a GameObject (for UI purposes mainly)
    /// </summary>
    /// <param name="obj">The GameObject of which you want to clear all children</param>
    public static void clearAllChildrenOfObj(GameObject obj) {
        for (int i = obj.transform.childCount - 1; i > -1; i--) {
            GameObject objToDelete = obj.transform.GetChild(i).gameObject;
            Destroy(objToDelete);
        }
    }



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


    public void GameOver() {
        Debug.Log("Game lost");    

    }  
}