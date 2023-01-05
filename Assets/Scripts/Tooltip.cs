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
	}

	private void Update() {
		//Only follow around the mouse if the gameobject is active, wouldn't wanna cause extra overhead in game
		if (gameObject.activeSelf) {
			//Input.mousePosition has (0,0) and the bottom left corner and the rest are screen coordinates
			Vector2 localPoint;
			//Convert screen coordinates to local coordinates within overarching rectangle
			//Since canvas uses screen space overlay, we set the screen camera to null since we don't have one
			RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
			transform.localPosition = localPoint; //Maybe see if we can linearly interpolate this
		}
	}


	public void ShowTooltip(string tooltipString) {
		gameObject.SetActive(true);
		m_TextMeshProUGUI.text = tooltipString;
		float extraTextPadding = 16f; //We set the offset to 4 in the editor but we can change it here and there to something else
		
		//Fix resizing of textbox height when we go from small text to big text
		/*
		    Height is calculated based on the current width,
		    so when we go to a bigger text, the text would
		    need to be wrapped on multiple lines keeping with
		    the current width. Changing the width and then
		    recalculating the height with the current width
		    would be smarter
		    
		    So try calculating the same size values twice
		*/
		for (int i = 0; i < 2; i++) {
		    //Get size of background image from size of text plus padding on both sides
		    Vector2 tooltipBackgroundSize = new Vector2(m_TextMeshProUGUI.preferredWidth + extraTextPadding * 2f, m_TextMeshProUGUI.preferredHeight + extraTextPadding * 2f);
		    m_BackgroundImageRectTransform.sizeDelta = tooltipBackgroundSize;
	    	m_TextRectTransform.sizeDelta = new Vector2(m_TextMeshProUGUI.preferredWidth, m_TextMeshProUGUI.preferredHeight);
		}
	}
	public void HideTooltip() {
		gameObject.SetActive(false);
	}
}