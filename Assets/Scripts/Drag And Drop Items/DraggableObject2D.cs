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
		canvasGroup = (CanvasGroup)gameObject.AddComponent<CanvasGroup>();
	}
	
	public virtual void OnBeginDrag(PointerEventData eventData) {
		Debug.Log("BeginDrag");
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.75f;

		m_RectTransform.SetParent(twoDimensionsItemsContainerInCanvas.transform, true);

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

	/*
private Vector2 startPos = new Vector2(0, 0);

private float mouseZCoordinate;
private Vector3 mouseOffset;

void OnMouseDown() {
	Debug.Log("MouseDownOn2DSprite");

	canvasGroup.blocksRaycasts = false;
	canvasGroup.alpha = 0.75f;

	mouseZCoordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

	mouseOffset = gameObject.transform.position - GetMousePositionInWorldCoordinates();
}
private Vector3 GetMousePositionInWorldCoordinates() {
	Vector3 mousePos = Input.mousePosition;
	mousePos.z = 0;

	return Camera.main.ScreenToWorldPoint(mousePos);
}

private void OnMouseDrag() {
	transform.localPosition = GetMousePositionInWorldCoordinates();// + mouseOffset;

}

private void OnMouseUp() {
	Debug.Log("MouseUpOn2DSprite");
	canvasGroup.blocksRaycasts = true;
	canvasGroup.alpha = 1f;
}
private void Update() {
	//Vector3 pos = transform.position;
	//pos.y = 2;
	//transform.position = pos;

}
*/

}
