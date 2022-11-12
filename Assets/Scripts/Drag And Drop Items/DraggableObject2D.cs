using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*Labelling methods as virtual so that deriving classes can override them and add more functionality on the same behaviors */
public class DraggableObject2D : MonoBehaviour, IDraggableObject {
	private Canvas canvas;
	private GameObject twoDimensionsItemsContainerInCanvas;
	private CanvasGroup canvasGroup;
	private RectTransform m_RectTransform;
	private GameObject ring;

	private void Awake() {
		canvas = GameObject.FindObjectOfType<Canvas>(); //Store the canvas in the scene so we can get its scale factor
		twoDimensionsItemsContainerInCanvas = GameObject.FindGameObjectWithTag("2DItemsContainerInCanvas");

		m_RectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>(); //Try to get the canvas group before we make one so we don't keep making one
		if (canvasGroup == null) {
			canvasGroup = (CanvasGroup)gameObject.AddComponent<CanvasGroup>();
		}
	}
	
	public virtual void OnBeginDrag(PointerEventData eventData) {
		Debug.Log("BeginDrag");
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.75f;
		if (m_RectTransform != null) {
			m_RectTransform.SetParent(twoDimensionsItemsContainerInCanvas.transform, true);
		}

	}

	public virtual void OnDrag(PointerEventData eventData) {
		Debug.Log("Dragging");
		if (m_RectTransform != null) {
			m_RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; //Adds the change in mouse position to the canvas position of our object
			//If we can move the object 
		}
	}

	public virtual void OnEndDrag(PointerEventData eventData) {
		Debug.Log("EndDrag");
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;
	}

}
