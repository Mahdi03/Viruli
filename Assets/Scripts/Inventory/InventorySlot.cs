using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler {
	public Image container;
	public Image item;
	public GameObject countText;
	private TextMeshProUGUI countTextValue;
	private int count = 0;

	private void Awake() {
		countTextValue = countText.GetComponent<TextMeshProUGUI>();
		countText.SetActive(!(count < 2)); //Disable count text if there is 1 or none
	}

	[HideInInspector]
	public bool clicked = false;

	public void OnDrop(PointerEventData eventData) {
		GameObject itemDroppedIntoSlot = eventData.pointerDrag; //Get actual object that was dropped on this current object
		if (itemDroppedIntoSlot != null) {
			//Ok we need to check whether we can drop the current object into this slot


			//Set its canvas relative position to the same position as this container (snap effect)
			count++;
			countText.SetActive(!(count < 2)); //Disable count text if there is 1 or none
			itemDroppedIntoSlot.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
		}
	}
}
