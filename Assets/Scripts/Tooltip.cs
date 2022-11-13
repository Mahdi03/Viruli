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
		
		
		ShowTooltip("helllllllllllo");
	}

	public void Update() {
		//Vector2 localPoint;
		//transform.position = Input.mousePosition;
		//RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, m_Camera, out localPoint);
		//transform.localPosition = localPoint;
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
