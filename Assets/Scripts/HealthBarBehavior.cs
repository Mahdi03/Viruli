using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*This script is attached to the parent canvas */
public class HealthBarBehavior : MonoBehaviour {

	private Slider slider;
	private Vector3 healthBarPosOffset;
	private bool enemy;
	private bool player;

	private Color playerLowHealthColor = Color.red;
	private Color playerHighHealthColor = Color.green;
	private Color enemyHealthColor = Color.blue;
	private Color potionTimeoutColor = Color.white;
	public bool isPotionTimeoutBar = false;

	public void SwitchCanvasToCamera() {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        //canvas.worldCamera = GameObject.FindGameObjectWithTag("MiniMapPreviewCamera").GetComponent<Camera>();	
    }
    public void SwitchCanvasToScreenSpaceOverlay() {
        Canvas canvas = GetComponent<Canvas>();
		canvas.renderMode= RenderMode.ScreenSpaceOverlay;
    }
	// Start is called before the first frame update
	void Start() {

		slider = transform.GetChild(0).GetComponent<Slider>(); //Actually get the slider component from the canvas' only child

		//Decide whether this is the health bar for the door or for any of the enemies
		if (transform.parent.name.ToLower().Contains("door")) {
			player = true;
			healthBarPosOffset = new Vector3(0, 5, 0);
		}
		else {
			player = false;
            healthBarPosOffset = new Vector3(0, 2, 0); //Set to this for all enemies
            if (isPotionTimeoutBar) {
                healthBarPosOffset = new Vector3(0, 8, 0); //Set to this for all potion timers
            }
		}
		enemy = !player;
		if (enemy) {
			
		}
        //Keep health bars hidden until we need to show them
        slider.gameObject.SetActive(false);
    }
	/*
	//Overloading just in case
	public void UpdateHealthBar(float health, int maxHealth) {
		UpdateHealthBar((int)health, maxHealth);
	}
	*/
	public void UpdateHealthBar(float health, float maxHealth) {
		if (!slider) {
			Start(); //Call just in case it hasn't been called yet
		}

		slider.gameObject.SetActive(health < maxHealth); //Start showing the health bar only if the item starts to take damage
		
		slider.value = health;
		slider.maxValue = maxHealth;

		//Choose two color schemas for health bar depending on whether the attached object is the player or the enemies
		if (player) {
			//Lerp is a linear interpolization between two provided colors a certain percentage of the way
			slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(playerLowHealthColor, playerHighHealthColor, slider.normalizedValue);
		}
		else if (isPotionTimeoutBar) {
			slider.fillRect.GetComponentInChildren<Image>().color = potionTimeoutColor;
        }
		else {
			//Must be enemy
			slider.fillRect.GetComponentInChildren<Image>().color = enemyHealthColor;
		}
	}
	public bool regularHealthBar = true;
	// Update is called once per frame
	void Update() {
		if (regularHealthBar) {
			slider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + healthBarPosOffset);
		}
		if (isPotionTimeoutBar) {
			slider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + healthBarPosOffset);
        }
	}
}
