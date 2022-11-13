using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour {
	private RectTransform m_BackgroundImageRectTransform;
	
	private RectTransform m_TextRectTransform;
	private TextMeshProUGUI m_TextMeshProUGUI;
	
	
	private void Awake() {
		Transform backgroundImage = transform.Find("Image");
		m_BackgroundImageRectTransform = backgroundImage.GetComponent<RectTransform>();

		Transform text = backgroundImage.Find("Text"); //Text is a child of the background image
		m_TextRectTransform = text.GetComponent<RectTransform>();
		m_TextMeshProUGUI = text.GetComponent<TextMeshProUGUI>();
		
		
		ShowTooltip("Potion #1");
	}

	public void Update() {
		//Input.mousePosition has (0,0) and the bottom left corner and the rest are screen coordinates
		Debug.Log(Input.mousePosition);
		Vector2 localPoint;
		//Convert screen coordinates to local coordinates within overarching rectangle
		//Since canvas uses screen space overlay, we set the screen camera to null since we don't have one
		RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
		transform.localPosition = localPoint;
	}


	private void ShowTooltip(string tooltipString) {
		gameObject.SetActive(true);
		m_TextMeshProUGUI.text = tooltipString;
		float extraTextPadding = 16f; //We set the offset to 4 in the editor but we can change it here and there to something else
		//Get size of background image from size of text plus padding on both sides
		Vector2 tooltipBackgroundSize = new Vector2(m_TextMeshProUGUI.preferredWidth + extraTextPadding * 2f, m_TextMeshProUGUI.preferredHeight + extraTextPadding * 2f);
		m_BackgroundImageRectTransform.sizeDelta = tooltipBackgroundSize;
		m_TextRectTransform.sizeDelta = new Vector2(m_TextMeshProUGUI.preferredWidth, m_TextMeshProUGUI.preferredHeight);
	}
	private void HideTooltip() {
		gameObject.SetActive(false);
	}
}
