using System;
using System.Collections.Generic;
using UnityEngine;

public class MainDoorManager : MonoBehaviour {

    private static MainDoorManager instance;
    public static MainDoorManager Instance { get { return instance; } }

    private List<MainDoor> mainDoors;

    [SerializeField]
    private GameObject doorAttackedNoises;
    private AudioSource[] doorAttackedAudioSources;
    [SerializeField]
    private GameObject doorBreakNoise;
    private AudioSource doorBreakAudioSource;
    //public List<AudioSouce> audioClipList { get; private set; } = new List<AudioClip>();

    private void Start() {
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            //Initialization code goes here
            mainDoors = InGameItemsDatabaseManager.Instance.mainDoors;
            doorAttackedAudioSources = doorAttackedNoises.GetComponents<AudioSource>();
            doorBreakAudioSource = doorBreakNoise.GetComponent<AudioSource>();
        }
    }

    public void PlayDoorBreakNoise() {
        doorBreakAudioSource.Play();
    }

    public void PlayRandomDoorAttackNoise() {
        int indexOfRandom = UnityEngine.Random.Range(0, doorAttackedAudioSources.Length);
        doorAttackedAudioSources[indexOfRandom].Play();
    }

    public void UnglowAllDoors() {
        foreach (var mainDoor in mainDoors) {
            //First make sure to Unglow all doors
            try {
                mainDoor.getDoorController().UnglowDoor();
            }
            catch (NullReferenceException e) {
                throw;
            }
        }
    }

    public void GlowDoorByID(int doorID) {
        UnglowAllDoors();
        mainDoors[doorID].getDoorController().GlowDoor();
    }
    public void UpgradeDoorByID(int doorID) {
        mainDoors[doorID].UpgradeDoor();
    }

}
