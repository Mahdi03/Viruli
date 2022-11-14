using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPSystemUIController : MonoBehaviour {
    [SerializeField]
    private GameObject levelTextbox;
    private TextMeshProUGUI levelTextboxText;

    [SerializeField]
    private GameObject xpBar;
    private Slider xpBarSlider;
    private Color xpBarColor = Color.cyan;

    [SerializeField]
    private GameObject currentXPTextbox;
    private TextMeshProUGUI currentXPTextboxText;
    
    private int level = -1, currentXp = -1, maxXp = -1;

    private void Awake() {
        levelTextboxText = levelTextbox.GetComponent<TextMeshProUGUI>();

        xpBarSlider = xpBar.GetComponent<Slider>();
        xpBarSlider.fillRect.GetComponentInChildren<Image>().color = xpBarColor; //Set color

        currentXPTextboxText = currentXPTextbox.GetComponent<TextMeshProUGUI>();

        this.updateUI();
    }


    public void updateXPSystemValuesInUI(int Level, int XP, int maxXP) {
        this.level = Level;
        this.currentXp = XP;
        this.maxXp = maxXP;
        this.updateUI();
    }

    private void updateUI() {
        if (levelTextboxText != null && xpBarSlider != null) {
            //Update level text w/ new level info
            levelTextboxText.text = "Level " + this.level;
            //Update xpBar w/ new xp info
            xpBarSlider.maxValue = this.maxXp; //Make sure to set max value before we set main value or else slider freaks out
            xpBarSlider.value = this.currentXp;
            
            
            //xpBarSlider.fillAmount = currentMana / maxMana;
            //Update currentXPText w/ new xp info
            currentXPTextboxText.text = this.currentXp + "/" + this.maxXp;
        }
        else {
            Debug.LogError("XPSystemUIController not yet initialized");
        }
    }
}
