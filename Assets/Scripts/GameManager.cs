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
            //ClearAllGameSettings();

        }
    }
    private void Start() {

        //Let's load the sound settings if they exist and apply them to the sound
        (float musicVolume, float sfxVolume) = LoadGameSettings();
        if (musicVolume < 0 || sfxVolume < 0) {
            //Then just keep it at default since they haven't done anything to it yet
        }
        else {
            PauseMenuSliderBehavior pauseMenuController = pauseMenu.GetComponent<PauseMenuSliderBehavior>();
            pauseMenuController.SetMusicVolume(musicVolume);
            pauseMenuController.SetSFXVolume(sfxVolume);
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

    public void PauseGame() { //Set as public so that the pause button can use it
        this.IS_GAME_PAUSED = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeGame() { //Set as public so that the pause menu can use it
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

    private void UpdatePlayerSettingsPreferences() {

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


    private const string SaveDataPlayerPrefsKeyName = "Mahdi_Viruli_StoredGameData";
    private const string GameSettingsPlayerPrefsKeyName = "Mahdi_Viruli_GameSettings";

    [Serializable]
    private class SaveGameData {
        public int currentXP, XPLevel;
        public int roundsPlayed;
        public string currentInventoryJSONString;
        //TODO: add all the door info
        public string allDoorInfoJSONString;
    }

    [Serializable]
    private class GameSettings {
        public float musicVolume;
        public float sfxVolume;
    }

    /// <summary>
    /// Returns the values as floats so as to be applied to the game's audio mixer and also show up in the settings every time
    /// </summary>
    /// <returns>(musicVolume, sfxVolume)</returns>
    public (float, float) LoadGameSettings() {
        string result = PlayerPrefs.GetString(GameSettingsPlayerPrefsKeyName, "");
        if (result == "") {
            return (-1f, -1f);
        }
        else {
            GameSettings savedGameSettings = JsonUtility.FromJson<GameSettings>(result);
            return (savedGameSettings.musicVolume, savedGameSettings.sfxVolume);
        }
    }

    /// <summary>
    /// Call this function only on value set, not on value changing
    /// </summary>
    /// <param name="musicVolume"></param>
    /// <param name="sfxVolume"></param>
    public void SaveGameSettings(float musicVolume, float sfxVolume) {
        GameSettings gameSettings = new GameSettings();
        gameSettings.musicVolume = musicVolume;
        gameSettings.sfxVolume = sfxVolume;
        string gameSettingsJSONString = JsonUtility.ToJson(gameSettings);
        PlayerPrefs.SetString(GameSettingsPlayerPrefsKeyName, gameSettingsJSONString);
        PlayerPrefs.Save();
        //Not sure if we need to save if it will save on application quit anyways
        Debug.Log(gameSettingsJSONString);
    }


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

        PlayerPrefs.SetString(SaveDataPlayerPrefsKeyName, saveGameDataJSONString);
        PlayerPrefs.Save();
        Debug.Log(saveGameDataJSONString);

    }

    private bool previousSaveAvailable() {
        string result = PlayerPrefs.GetString(SaveDataPlayerPrefsKeyName, null);
        return (result != null);
    }

    public void LoadSavedGame() {
        if (previousSaveAvailable()) {
            //Load saved game data
            string result = PlayerPrefs.GetString(SaveDataPlayerPrefsKeyName, null);
            SaveGameData saveData = JsonUtility.FromJson<SaveGameData>(result);
            //Load XPSystem data
            XPSystem.Instance.LoadSaveData(saveData.XPLevel, saveData.currentXP);
            //EnemySpawner.Instance.rou


        }
    }

    public void ClearAllGameSettings() {
        PlayerPrefs.DeleteKey(GameSettingsPlayerPrefsKeyName);
    }

    public void ClearAllSaveData() {
        PlayerPrefs.DeleteKey(SaveDataPlayerPrefsKeyName);
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