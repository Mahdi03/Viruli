using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public GameObject continueButton;

    private void Start() {
        //Show/hide continue button depending on whether we have a previous game saved
        continueButton.SetActive(GameManager.previousSaveAvailable());
    }


    public void ContinueGame() {
        SceneManager.LoadScene("GameScene");
    }

    public void NewGame() {
        GameManager.ClearAllSaveData();
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
