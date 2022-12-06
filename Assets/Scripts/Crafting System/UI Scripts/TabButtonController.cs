using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButtonController : MonoBehaviour, IPointerClickHandler {
    public Color buttonTabActiveColor = Color.HSVToRGB(214f/360, 39f/100, 75f/100); //Color.white;
    public Color buttonTabInactiveColor = Color.HSVToRGB(228f/360, 27f/100, 53f/100); // Color.HSVToRGB(0, 0, 0.6f);

    public int tabID { get { 
            if (!initialized) { Awake(); }
            if (itemInstance == null) { itemInstance = GetComponent<ItemInstance>(); }
            return itemInstance.itemID;
        } }

	public void OnPointerClick(PointerEventData eventData) {
		//OnClick, select this tab
        CraftingUITabsManager.Instance.SelectTab(tabID);
	}
    private bool initialized = false;
	private Image padding = null;
    private ItemInstance itemInstance = null;
    private void Awake() {
        if (padding == null) {
            padding = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        }
        if (itemInstance == null) {
            itemInstance = GetComponent<ItemInstance>();
        }
        initialized= true;
    }
    public void UnselectButton() {
        if (!initialized) { Awake(); }
        padding.color = buttonTabInactiveColor;
    }
	public void SelectButton() {
        if (!initialized) { Awake(); }
        padding.color = buttonTabActiveColor;
    }

}
