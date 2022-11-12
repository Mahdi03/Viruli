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

    public void ShowFullInventory() {
        gameObject.SetActive(true);
        //Now refresh the UI with all the elements
        InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
        //Every time they open the UI make sure to scroll to the top
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }

    // Start is called before the first frame update
    void Start() {
        //numOfInventorySlots = InventoryManager.Instance.getTotalItemsCount();
        //slotWidth = inventorySlotPrefab.GetComponent<Image>().rectTransform.sizeDelta.x; //Not that we need it tho
        fullInventoryContentContainer = GameObject.FindGameObjectWithTag("FullInventoryContent");
        for (int i = 0; i < 100; i++) {
            GameObject newSlot = Instantiate(inventorySlotPrefab, fullInventoryContentContainer.transform);
            //newSlot.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(currentSlotPos, 0);
            newSlot.GetComponent<InventorySlot>().slotID = (int)i;
        }
        InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();

        scrollRect = GetComponent<ScrollRect>();
    }

}
