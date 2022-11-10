using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickInventoryManager : MonoBehaviour {
	public GameObject inventorySlotPrefab;
	public int numOfQuickInventorySlots;
	private float slotWidth;

	// Start is called before the first frame update
	void Start() {
		//Set up a certain number of cells on the screen

		//Get width of each slot
		slotWidth = inventorySlotPrefab.GetComponent<Image>().rectTransform.sizeDelta.x;
		//Debug.Log(slotWidth);
		for (float i = -numOfQuickInventorySlots / 2f; i < numOfQuickInventorySlots / 2f; i++) {
			GameObject newSlot = Instantiate(inventorySlotPrefab, transform);
			newSlot.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(i * slotWidth, 0);
			newSlot.GetComponent<InventorySlot>().SetSlotID((int)i);
		}
	}

}
