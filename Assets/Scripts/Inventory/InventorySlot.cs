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
	private int slotID;
	private int itemID = -1;
	private int count = 0;

	private Vector2 currentSlotRectTransformAnchorMin, currentSlotRectTransformAnchorMax, currentSlotRectTransformAnchorPos;

	public void SetSlotID(int id) {
		slotID = id;
	}

    private void Awake() {
		countTextValue = countText.GetComponent<TextMeshProUGUI>();
		countText.SetActive(!(count < 2)); //Disable count text if there is 1 or none
		RectTransform rectTransform = GetComponent<RectTransform>();
        currentSlotRectTransformAnchorMin = rectTransform.anchorMin;
		currentSlotRectTransformAnchorMax = rectTransform.anchorMax;
		currentSlotRectTransformAnchorPos = rectTransform.anchoredPosition;
	}
	
	public void OnDrop(PointerEventData eventData) {
		GameObject itemDroppedIntoSlot = eventData.pointerDrag; //Get actual object that was dropped on this current object
		if (itemDroppedIntoSlot != null) {
			//Ok we need to check whether we can drop the current object into this slot
			//if itemID = item.id, add it to this one
			//else if itemID = -1, also add it to this one
			//else (there is another item here, let's insert this one here and find a new place for the current item)

			//Set its canvas relative position to the same position as this container (snap effect)
			count++;
			countText.SetActive(!(count < 2)); //Disable count text if there is 1 or none
			countTextValue.text = count.ToString();
			RectTransform rectTransformOfDroppedItem = itemDroppedIntoSlot.GetComponent<RectTransform>();
			rectTransformOfDroppedItem.SetParent(transform, false);
			rectTransformOfDroppedItem.anchorMin = currentSlotRectTransformAnchorMin;
            rectTransformOfDroppedItem.anchorMax = currentSlotRectTransformAnchorMax;
			rectTransformOfDroppedItem.anchoredPosition = currentSlotRectTransformAnchorPos;
			//itemDroppedIntoSlot.GetComponent<RectTransform>() = rectTransformOfDroppedItem;
			
		}
	}
}
