using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftableUIInfoGroupContainerController : MonoBehaviour {
    public GameObject prefabIconContainer;
    
    public GameObject itemNameTextbox;
    private TextMeshProUGUI itemNameTextboxText;
    
    public GameObject itemDescriptionTextbox;
    private TextMeshProUGUI itemDescriptionTextboxText;

    public GameObject itemStatsTextbox;
    private TextMeshProUGUI itemStatsTextboxText;

    private void Awake() {
        itemNameTextboxText = itemNameTextbox.GetComponent<TextMeshProUGUI>();
        itemDescriptionTextboxText = itemDescriptionTextbox.GetComponent<TextMeshProUGUI>();
        itemStatsTextboxText = itemStatsTextbox.GetComponent<TextMeshProUGUI>();
    }
    public void SetIcon(GameObject prefab) {
        GameManager.clearAllChildrenOfObj(prefabIconContainer);
        Instantiate(prefab, prefabIconContainer.transform);
    }

    public void SetItemName(string itemName) {
        itemNameTextboxText.text = itemName;
    }
    public void SetItemDescription(string itemDescription) {
        itemDescriptionTextboxText.text = itemDescription;
    }
    public void SetItemStatsText(string itemStatsText) {
        itemStatsTextboxText.text = itemStatsText;
    }
}
