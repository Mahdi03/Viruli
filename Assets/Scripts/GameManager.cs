using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static readonly int LAYER_DroppableGround = 1 << 3; //Bit shift by 3 to get the 3rd layer
    public static readonly int LAYER_MainDoor = 1 << 7; //MainDoor Layer is Layer 7
    public static readonly int LAYER_Enemy = 1 << 6; //Enemy layer is Layer 6

    public Canvas mainCanvas;

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    private AudioManager audioManager;

    public GameObject tooltipObjInScene;


    [SerializeField]
    private GameObject pauseMenu;


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
            audioManager = GetComponent<AudioManager>();

        }
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            if (pauseMenu.activeSelf) {
                //The pause menu is already open, let us resume game
                ResumeGame();
            }
            else {
                //We need to pause the game
                PauseGame();
            }
        }
    }

    //This global variable is what allows all the individual pieces to do a pause-check every frame whether they want ot pause the audio or not
    public bool IS_GAME_PAUSED { get; private set; } = false;

    private void PauseGame() {
        this.IS_GAME_PAUSED = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    private void ResumeGame() {
        this.IS_GAME_PAUSED = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }


    /*Audio Controls*/

    public void SetMasterVolume(float volume) {
        audioManager.SetMasterVolume(volume);
    }
    public void SetMusicVolume(float volume) {
        audioManager.SetMusicVolume(volume);
    }
    public void SetSoundEffectsVolume(float volume) {
        audioManager.SetSoundEffectsVolume(volume);
    }

    public GameObject GetTooltip() { return tooltipObjInScene; }

    private bool gameAlreadyLost = false;
    public void GameOver() {
        if (!gameAlreadyLost) {
            Debug.Log("Game lost");
            SceneManager.LoadScene("LoseGame", LoadSceneMode.Additive);
        }
    }

    [Serializable]
    public struct MainDoorSaveData {
        public MainDoorSaveData(string doorName, int currentLevel, int currentHealth) {
            this.doorName = doorName;
            this.currentLevel = currentLevel;
            this.currentHealth = currentHealth;
            
        }
        public string doorName;
        public int currentLevel;
        public int currentHealth;
    }
    

    [Serializable]
    private class SaveGameData {
        public int currentXP, XPLevel;
        public int roundsPlayed;
        public string currentInventoryJSONString;
        //TODO: add all the door info
        public string allDoorInfoJSONString;
    }

    private const string PlayerPrefsKeyName = "Mahdi_Viruli_StoredGameData";


    /// <summary>
    /// Provide global method to save all game data to player prefs
    ///
    /// Call this function between rounds to save game data
    /// </summary>
    public void SaveGame(int roundsPlayed = 0) {

        SaveGameData saveGameData = new SaveGameData();

        saveGameData.currentXP = XPSystem.Instance.XP;
        saveGameData.XPLevel = XPSystem.Instance.Level;
        saveGameData.roundsPlayed = roundsPlayed;
        saveGameData.currentInventoryJSONString = InventoryManager.Instance.GetCurrentInventoryJSONString();

        List<MainDoorSaveData> doorSaveDatas = new List<MainDoorSaveData>();

        foreach (MainDoor door in InGameItemsDatabaseManager.Instance.mainDoors) {
            MainDoorController doorController = door.getDoorController();
            (int currentDoorHealth, int maxDoorHealth) = doorController.getCurrentHealthStats();
            int currentDoorLevel = doorController.getLevel();
            //string doorName = doorController.gameObject.GetComponent<MainDoorInstance>().doorName;
            string doorName = door.name;
            MainDoorSaveData doorSaveData = new MainDoorSaveData(doorName, currentDoorLevel, currentDoorHealth);
            doorSaveDatas.Add(doorSaveData);
        }

        string doorDataJSON = JsonHelper.ToJson(doorSaveDatas.ToArray()); //Make sure to unpack with the MainDoorSaveData struct type
        saveGameData.allDoorInfoJSONString = doorDataJSON;


        //Now once again JSON serialize that data and then add it to PlayerPrefs
        string saveGameDataJSONString = JsonUtility.ToJson(saveGameData);

        PlayerPrefs.SetString(PlayerPrefsKeyName, saveGameDataJSONString);
        PlayerPrefs.Save();
        Debug.Log(saveGameDataJSONString);

    }

    private bool previousSaveAvailable() {
        string result = PlayerPrefs.GetString(PlayerPrefsKeyName, null);
        return (result != null);
    }

    public void LoadSavedGame() {
        if (previousSaveAvailable()) {
            //Load saved game data
            string result = PlayerPrefs.GetString(PlayerPrefsKeyName, null);
            SaveGameData saveData = JsonUtility.FromJson<SaveGameData>(result);
            //Load XPSystem data
            XPSystem.Instance.LoadSaveData(saveData.XPLevel, saveData.currentXP);
            //EnemySpawner.Instance.rou


        }
    }

    public void ClearAllSaveData() {
        PlayerPrefs.DeleteKey(PlayerPrefsKeyName);
    }
    public void RestartGame() {
        //Start game afresh
        ClearAllSaveData();
        PlayGame();
    }

    public void PlayGame() {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame() {
        Application.Quit();
    }

}