using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FullInventoryButton : MonoBehaviour, IPointerDownHandler {
	private GameObject fullInventory;
	private FullInventoryManager fullInventoryManager;
	public void init(GameObject obj) {
		fullInventory = obj; //We'll have to pass it on from the object that creates this object during runtime
		fullInventoryManager = obj.GetComponent<FullInventoryManager>();
	}
	public void OnPointerDown(PointerEventData eventData) {
		//If full inventory is already displayed then hide it
		if (fullInventoryManager.active) {
			fullInventory.SetActive(false);
		}
		else {
			//We were clicked on, now display full inventory
			fullInventoryManager.ShowFullInventory();
		}
	}
}
