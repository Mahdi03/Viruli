using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButtonController : MonoBehaviour, IPointerClickHandler {
    private Color buttonTabBorderActiveColor = Color.HSVToRGB(180f/360, 100f/100, 100f/100); //Sharp blue
    private Color buttonTabBorderInactiveColor = Color.HSVToRGB(0f, 0f, 30f/100); //Gray
    private Color buttonTabActiveColor = Color.HSVToRGB(214f/360, 39f/100, 95f/100); //Color.white;
    private Color buttonTabInactiveColor = Color.HSVToRGB(228f/360, 27f/100, 43f/100); // Color.HSVToRGB(0, 0, 0.6f);
    //private Color buttonTabInactiveColor = Color.HSVToRGB(210f / 360, 32f / 100, 27f / 100); // Color.HSVToRGB(0, 0, 0.6f);

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
    [SerializeField]
    private Image border;
    [SerializeField]
	private Image padding;
    private ItemInstance itemInstance = null;
    private void Awake() {
        /*
        if (border == null) {
            border = transform.GetChild(0).GetComponent<Image>();
        }
        if (padding == null) {
            padding = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        }
        */
        if (itemInstance == null) {
            itemInstance = GetComponent<ItemInstance>();
        }
        initialized = true;
    }
    public void UnselectButton() {
        if (!initialized) { Awake(); }
        border.color = buttonTabBorderInactiveColor;
        padding.color = buttonTabInactiveColor;
    }
	public void SelectButton() {
        if (!initialized) { Awake(); }
        border.color = buttonTabBorderActiveColor;
        padding.color = buttonTabActiveColor;
    }

}
