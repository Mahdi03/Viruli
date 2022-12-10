using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseGameScreenFade : MonoBehaviour {

    private Image image;
    
    private void Awake() {
        image = GetComponent<Image>();
        GameManager.Instance.SetSoundEffectsVolume(0); //Mute game sounds in background for the time being
    }

    private Color transparent = Color.clear;
    private Color blackFadeOut = new Color(0, 0, 0, 0.55f);
    public bool updateBackground = true;
    float i = 0;
    private void Update() {
        if (updateBackground) {
            i += Time.smoothDeltaTime;
            image.color = Color.Lerp(transparent, blackFadeOut, i);
        }
    }

    public void RestartFromLastCheckpoint() {
        GameManager.Instance.PlayGame();
    }

    public void RestartGame() {
        GameManager.Instance.RestartGame();
    }

    public void QuitGame() {
        GameManager.Instance.QuitGame();
    }


}
