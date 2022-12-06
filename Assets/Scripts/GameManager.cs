using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static readonly int LAYER_DroppableGround = 1 << 3; //Bit shift by 3 to get the 3rd layer
	public static readonly int LAYER_MainDoor = 1 << 7; //MainDoor Layer is Layer 7
	public static readonly int LAYER_Enemy = 1 << 6; //Enemy layer is Layer 6

	public Canvas mainCanvas;

	private static GameManager instance;
	public static GameManager Instance { get { return instance; } }

	public GameObject tooltipObjInScene;


    /*Fonts*/
    public TMP_FontAsset CRAFTINGUI_regularTextFont;
    public TMP_FontAsset CRAFTINGUI_costTextFont;



    public static void clearAllChildrenOfObj(Transform obj) {
		clearAllChildrenOfObj(obj.gameObject);
	}
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
		//TODO: Implement game lose

	}
}