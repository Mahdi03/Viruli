using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

interface IDraggableObject : IPointerDownHandler,
	/*Drag Events*/
	IBeginDragHandler, IEndDragHandler, //For begin and end drag events
	IDragHandler //For while dragging (called each frame)
{}


/*Labelling methods as virtual so that deriving classes can override them and add more functionality on the same behaviors */
public class DraggableObject : MonoBehaviour, IDraggableObject {
	private Canvas canvas;
	private CanvasGroup canvasGroup;
	private RectTransform m_RectTransform;
	private void Awake() {
		canvas = GameObject.FindObjectOfType<Canvas>(); //Store the canvas in the scene so we can get its scale factor
		m_RectTransform = GetComponent<RectTransform>();
		canvasGroup = (CanvasGroup)gameObject.AddComponent<CanvasGroup>();
	}
	public virtual void OnBeginDrag(PointerEventData eventData) {
		Debug.Log("BeginDrag");
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.75f;
	}

	public virtual void OnDrag(PointerEventData eventData) {
		Debug.Log("Dragging");
		if (m_RectTransform != null) {
			m_RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; //Adds the change in mouse position to the canvas position of our object
		}
	}

	public virtual void OnEndDrag(PointerEventData eventData) {
		Debug.Log("EndDrag");
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;
	}

	public virtual void OnPointerDown(PointerEventData eventData) {
		Debug.Log("PointerDown");
	}
}