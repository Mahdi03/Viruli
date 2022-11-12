using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickInventoryManager : MonoBehaviour {
	public GameObject inventorySlotPrefab;
	public GameObject fullInventoryButton;
	public GameObject fullInventory;
	public int numOfQuickInventorySlots;
	private float slotWidth;

	// Start is called before the first frame update
	void Start() {
		//Set up a certain number of cells on the screen

		//Get width of each slot
		slotWidth = inventorySlotPrefab.GetComponent<Image>().rectTransform.sizeDelta.x;
		//Debug.Log(slotWidth);
		int i = 0;
		for (i = 0; i < numOfQuickInventorySlots; i++) {
			float currentSlotPos = (i - numOfQuickInventorySlots / 2f) * slotWidth;
			//Debug.Log("a:" + currentSlotPos);
			
			GameObject newSlot = Instantiate(inventorySlotPrefab, transform);
			newSlot.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(currentSlotPos, 0);
			newSlot.GetComponent<InventorySlot>().slotID = (int)i;
			
		}
		//Now i is a value we can use to add our button prefab to open the entire inventory
		GameObject button = Instantiate(fullInventoryButton, transform);
        button.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2((i - numOfQuickInventorySlots / 2f) * slotWidth, 0);
		button.GetComponent<FullInventoryButton>().init(fullInventory); //Pass on the value from the unity editor to this new button
        //newSlot.GetComponent<InventorySlot>().slotID = (int)i;
        /*
		for (float i = -numOfQuickInventorySlots / 2f; i < numOfQuickInventorySlots / 2f; i++) {
			Debug.Log("b:" + i * slotWidth);
			
			GameObject newSlot = Instantiate(inventorySlotPrefab, transform);
			newSlot.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(i * slotWidth, 0);
			newSlot.GetComponent<InventorySlot>().SetSlotID((int)i);
		}
		*/
    }

}
