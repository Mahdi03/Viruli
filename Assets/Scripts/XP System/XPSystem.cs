using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class XPSystem : MonoBehaviour {

    private static XPSystem instance;
    public static XPSystem Instance { get { return instance; } }

    //Public global variables accessible everywhere
    public int Level { get; private set; } = 0;
    public int XP { get; private set; } = 0;
    public int MaxXP { get; private set; }

    private bool alreadyInstantiatedData = false;

    //Stored as index
    private List<int> xpThresholds = new List<int>()
    {
        100, //Level 0->1
		100+150, //Level 1->2
		100+150+300, //Level 2->3
		100+150+300+500, //Level 3->4
		100+150+300+500+750, //Level 4->5
		100+150+300+500+750+1000, //Level 5->6
		100+150+300+500+750+1000+1500, //Level 6->7
		100+150+300+500+750+1000+1500+2250, //Level 7->8
        100+150+300+500+750+1000+1500+2250+3500, //Level 8->9
        100+150+300+500+750+1000+1500+2250+3500+4000, //Level 9->10
        100+150+300+500+750+1000+1500+2250+3500+4000+6000, //Level 10->11
        100+150+300+500+750+1000+1500+2250+3500+4000+6000+10000, //Level 11->12
	};


    private void Awake() {
        //Singleton initialization code
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            //Now we can initialize stuff
            this.MaxXP = this.xpThresholds[this.Level];

            //TODO: Initialize the level xp thresholds using a recursively-defined function

            //Update UI immediately to get started
            this.updateXPUI();
        }
    }

    public void LoadSaveData(int level, int currentXP) {
        this.Level = level;
        this.XP = currentXP;

        this.MaxXP = this.xpThresholds[this.Level];
        this.updateXPUI();
    }

    public void increaseXP(int xp) {
        this.XP += xp;

        //Check if update matches level threshold
        if (this.XP >= this.xpThresholds[this.Level]) { //Make it so that they never reach a full 100%
            if (this.Level + 1 < this.xpThresholds.Count) { //Only increase the level if we have another level to go
                this.Level++; //Increase the level
                MessageSystem.Instance.PostMessage("Congrats! You have now leveled up to XP Level " + this.Level + ".");
                if (this.Level == 4) {
                    MessageSystem.Instance.PostMessage("You have now unlocked Level 2 spells!", alert: true);
                }
                else if (this.Level == 9) {
                    MessageSystem.Instance.PostMessage("You have now unlocked Level 3 spells!", alert: true);
                }
            }

            this.MaxXP = this.xpThresholds[this.Level]; //Update the new XP max

        }
        //Indicate changes in XP and Level into the UI
        this.updateXPUI();
    }
    public void decreaseXP(int xp) {
        this.XP = Mathf.Max(this.XP - xp, 0); //Limit the xp to drop to 0 and no more

        //We don't need to deal with level stuff, once you level up you level up

        //Indicate changes in XP and Level into the UI
        this.updateXPUI();
    }
    public void updateXPUI() {
        GameObject[] xpSystemPrefabs = GameObject.FindGameObjectsWithTag("xpSystemUI");
        foreach (GameObject currentXPSystemPrefab in xpSystemPrefabs) {
            XPSystemUIController xpUIController = currentXPSystemPrefab.GetComponent<XPSystemUIController>();
            if (xpUIController != null) {
                xpUIController.updateXPSystemValuesInUI(this.Level, this.XP, this.MaxXP);
            }
            else {
                throw new System.Exception("XPSystemUIController not attached to item");
            }
        }
    }

}
