using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StorylineSceneController : MonoBehaviour {
    public void PlayGame() {
        SceneManager.LoadScene("GameScene");
    }
}
