using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TabButtonController : MonoBehaviour, IPointerDownHandler {
	public GameObject thisTabsContent;
	public Color buttonTabActiveColor = Color.white;
	public Color buttonTabInactiveColor = Color.HSVToRGB(0, 0, 0.6f);

	public void OnPointerDown(PointerEventData eventData) {
		//OnClick, disable the other tabs and then show the content related to this tab
	}
}
