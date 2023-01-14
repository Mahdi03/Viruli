using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    public GameObject continueButton;

    private void Start() {
        //Show/hide continue button depending on whether we have a previous game saved
        continueButton.SetActive(GameManager.previousSaveAvailable());
        //continueButton.GetComponent<Button>().interactable = GameManager.previousSaveAvailable();
    }


    public void ContinueGame() {
        SceneManager.LoadScene("GameScene");
    }

    public void NewGame() {
        GameManager.ClearAllSaveData();
        SceneManager.LoadScene("StorylineScene");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
