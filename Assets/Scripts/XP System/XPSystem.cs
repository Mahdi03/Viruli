using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class XPSystem : MonoBehaviour {
	private static XPSystem instance;
	public static XPSystem Instance { get { return instance; } }

	//Public global variables accessible everywhere
	public int Level { get; private set; }
	public int XP { get; private set; }
	public int MaxXP { get; private set; }

	//Stored as index
	private List<int> xpThresholds = new List<int>()
	{
		100 //Level 0->1
	};
	
	private void Awake() {
		//Singleton initialization code
		if (instance != this && instance != null) {
			Destroy(gameObject);
		}
		else {
			instance = this;
			//Now we can initialize stuff
			this.Level = 0; //TODO: Load from PlayerPrefs
			this.XP = 30; //TODO: Load from PlayerPrefs
			this.MaxXP = this.xpThresholds[this.Level];

			//Initialize the level xp thresholds using a recursively-defined function

			//Update UI immediately to get started
			this.updateXPUI();
		}
	}
	public void increaseXP(int xp) {
		this.XP += xp;

		//Check if update matches level threshold
		if (this.XP > this.xpThresholds[this.Level]) {
			this.Level++; //Increase the level
            this.MaxXP = this.xpThresholds[this.Level]; //Update the new XP max
        }
		//Indicate changes in XP and Level into the UI
		this.updateXPUI();
	}

	


	public void updateXPUI() {
		GameObject[] xpSystemPrefabs = GameObject.FindGameObjectsWithTag("xpSystemUI");
		foreach (GameObject currentXPSystemPrefab in xpSystemPrefabs) {
			XPSystemUIController xpUIController = currentXPSystemPrefab.GetComponent<XPSystemUIController>();
			if (xpUIController != null) {
				xpUIController.updateXPSystemValuesInUI(this.Level, this.XP, this.MaxXP);
			} else {
				throw new System.Exception("XPSystemUIController not attached to item");
			}
		}
	}

}
