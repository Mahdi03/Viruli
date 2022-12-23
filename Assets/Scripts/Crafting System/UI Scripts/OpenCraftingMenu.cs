using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class will be attached to any 2D or 3D object and make it clickable
/// OnClick it will simply show the Crafting Menu from its last state (the 
/// crafting menu can be closed with a separate close button that is defined in
/// the editor UI)
/// </summary>
public class OpenCraftingMenu : MonoBehaviour, IPointerDownHandler {
	[SerializeField]
	private GameObject craftingMenu;


	//In case it is a 2-D object
	public void OnPointerDown(PointerEventData eventData) {
		showCraftingMenu();
	}

	//In case it is a 3-D object
	private void OnMouseDown() {
		showCraftingMenu();
	}

	public void showCraftingMenu() {
		if (!EnemySpawner.Instance.currentlyInRoundBreak) {
			Time.timeScale = 0f; //Pause game altogether in the background so that the user can focus more on the crafting UI
		}
		
		craftingMenu.SetActive(true);
		XPSystem.Instance.updateXPUI(); //Remember to update the XP every time we open the crafting menu
		//TODO: Update the recipe table UI for both crafting potions and 
		switch (CraftingUITabsManager.Instance.activeTabID) {
			case 0: {
					//We are in the potions UI, deal with the potion UI
					if (CraftingUIPotionsManager.Instance.itemID != -1) {
						//Then we have an item that is actively selected, update the UI
						int amountToCraft = CraftingUIPotionsManager.Instance.amountToCraft;
                        CraftingUIPotionsManager.Instance.UpdateCraftingRecipeTable(amountToCraft);
					}
				}
				break;
			case 1: {
					if (CraftingUIDoorsManager.Instance.doorID != -1) {
						//Then we have a door actively selected, update the recipe tables
						CraftingUIDoorsManager.Instance.LoadDoorUI();
						//Actively glow the door that is actively selected
						MainDoorManager.Instance.GlowDoorByID(CraftingUIDoorsManager.Instance.doorID);

					}
					//We are in the door repair/upgrade UI, deal with door UI
                    //CraftingUIDoorsManager.Instance.resumeCoroutines(); //Game is paused when crafting menu is opened, we don't need to have a coroutine
                }
                break;
		}
		
	}
}
