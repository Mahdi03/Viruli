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
		craftingMenu.SetActive(true);
	}
}
