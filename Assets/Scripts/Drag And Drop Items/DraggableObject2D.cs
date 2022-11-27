using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*Labelling methods as virtual so that deriving classes can override them and add more functionality on the same behaviors */
public class DraggableObject2D : MonoBehaviour, IDraggableObject2D {
	private Canvas canvas;
	private GameObject twoDimensionsItemsContainerForDraggingInCanvas;
	private CanvasGroup canvasGroup;
	private RectTransform m_RectTransform;

	private void Awake() {
		canvas = GameManager.Instance.mainCanvas; //Store the canvas in the scene so we can get its scale factor
		twoDimensionsItemsContainerForDraggingInCanvas = GameObject.FindGameObjectWithTag("2DItemsContainerForDraggingInCanvas");

		m_RectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>(); //Try to get the canvas group before we make one so we don't keep making one
		if (canvasGroup == null) {
			canvasGroup = (CanvasGroup)gameObject.AddComponent<CanvasGroup>();
		}
	}

	public virtual void OnBeginDrag(PointerEventData eventData) {
		//Debug.Log("BeginDrag");
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.75f;
		if (m_RectTransform != null) {
			m_RectTransform.SetParent(twoDimensionsItemsContainerForDraggingInCanvas.transform, true);
		}

	}

	public virtual void OnDrag(PointerEventData eventData) {
		//Debug.Log("Dragging");
		if (m_RectTransform != null) {
			m_RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; //Adds the change in mouse position to the canvas position of our object
		}
	}

	private bool overInventoryUI;

	public virtual void OnEndDrag(PointerEventData eventData) {
		//Debug.Log("EndDrag");
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;

		if (GetComponent<DroppableObject3D>() == null) {
            //As it turns out, if run this code then that means the drop never took place which means we landed in a non-droppable place so we can just cancel the drop
            InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
            Destroy(gameObject);

            //Ok we are only a 2D draggable object, not droppable3D is covering us
            //We need to check if we are dropping over UI or if we need to cancel the drag event
            overInventoryUI = false;
			//Here we need to check whether we are on top of the inventory UI before we decide anything else
			GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();

			List<RaycastResult> results = new List<RaycastResult>();
			gr.Raycast(eventData, results);
			foreach (RaycastResult result in results) {
				overInventoryUI = true;
			}
			if (!overInventoryUI) {
                //We need to cancel the drop
                InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
                Destroy(gameObject);
            }
		}
	}

}
