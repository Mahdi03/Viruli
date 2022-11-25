using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullInventoryManager : MonoBehaviour {
	public GameObject inventorySlotPrefab;
	private int numOfInventorySlots;
	private float slotWidth;
	private GameObject fullInventoryContentContainer;
	private ScrollRect scrollRect;

	public bool active { get { return gameObject.activeSelf; } }

	public void ShowFullInventory() {
		gameObject.SetActive(true);
		//Now refresh the UI with all the elements
		InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
		//We'll use a null check here because the object might not have fully instantiated just yet when we open the inventory for the first time but that's ok because it'll start at the top anyways
		if (scrollRect != null) {
			//Every time they open the UI make sure to scroll to the top
			scrollRect.normalizedPosition = new Vector2(0, 1);
		}
	}

	// Start is called before the first frame update
	void Start() {
		fullInventoryContentContainer = GameObject.FindGameObjectWithTag("FullInventoryContent");
		//TODO: Have a dynamically increasing FullInventory that will add more rows to accomodate the more items we have
		for (int i = 10; i < 100; i++) {
			GameObject newSlot = Instantiate(inventorySlotPrefab, fullInventoryContentContainer.transform);
			newSlot.GetComponent<InventorySlot>().slotID = (int)i;
		}
		InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();

		scrollRect = GetComponent<ScrollRect>();
	}

}
