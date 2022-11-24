using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This component will use the ItemInstance to get the itemID and necessary prefabs to instantiate
/// </summary>
[RequireComponent(typeof(ItemInstance))]
[RequireComponent(typeof(CanvasGroup))]
public class DroppableObject3D : MonoBehaviour, IDraggableObject2D {
	private Canvas canvas;
	private GameObject threeDimensionalItemsContainerForDraggingInWorldSpace;
	private CanvasGroup canvasGroup;
	private int itemID;
	private IItem item;
	private GameObject threeDimensionalPrefab;
	
	private void Awake() {
		canvas = GameObject.FindObjectOfType<Canvas>(); //Store the canvas in the scene so we can get its dimensions
		threeDimensionalItemsContainerForDraggingInWorldSpace = GameObject.FindGameObjectWithTag("threeDimensionalItemsContainerForDraggingInWorldSpace");

        canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null) {
			canvasGroup = (CanvasGroup)gameObject.AddComponent<CanvasGroup>();
		}
		this.itemID = GetComponent<ItemInstance>().itemID;
		this.item = InGameItemsDatabaseManager.Instance.getItemByID(itemID);
	}



	public void OnBeginDrag(PointerEventData eventData) {
		//Let's instantiate a new 3D spell that we can show and hide depending on where we are dragging
		if (threeDimensionalPrefab == null) {
			threeDimensionalPrefab = Instantiate(item.ThreeDimensionalPrefab);
			IItem.attachItemInstance(threeDimensionalPrefab, itemID);
			IItem.allowHoverTooltip(threeDimensionalPrefab);
			threeDimensionalPrefab.transform.SetParent(threeDimensionalItemsContainerForDraggingInWorldSpace.transform, true);
		}
		threeDimensionalPrefab.SetActive(false);
		//Right now it is just there for looks, but we will activate it when we drop it
	}


	private Vector2 getViewportCoordinatesOf2DRectTransform(RectTransform rectTransform) {
		//rectTransform.position gives us screen coordinates
		
		RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
		Vector2 canvasDimensions = new Vector2(canvasRectTransform.rect.width, canvasRectTransform.rect.height) * canvas.scaleFactor;

		//Normalize screen coordinates to get viewport coordinates
		Vector2 viewportCoordinates = new Vector2(
			x:rectTransform.position.x / canvasDimensions.x,
			y:rectTransform.position.y / canvasDimensions.y
			);
		return viewportCoordinates;
	}

	public void OnDrag(PointerEventData eventData) {
		RectTransform rectTransformOfDraggableObj = eventData.pointerDrag.GetComponent<RectTransform>();
		//We don't need to check if we are over the inventory or anything like that because we are only in the middle of a drag event right now
		
		Vector2 viewportCoordinates = getViewportCoordinatesOf2DRectTransform(rectTransformOfDraggableObj);

		//Send a ray through that viewport point, if we hit the ground layermask then we can continue, else we need to disable
		Ray ray = Camera.main.ViewportPointToRay(viewportCoordinates);
		
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000f, GameManager.LAYER_DroppableGround)) { //Use bit shift by 3 to get 3rd layer
			//Then we hit something in the droppable ground
			//Debug.Log("Hovering over ground");
			canvasGroup.alpha = 0f; //Make it completely transparent so that it APPEARS we are dragging the 3D object
			//Get distance to hit and make that our distance from camera
			var worldPointAtGround = hit.point;
			threeDimensionalPrefab.SetActive(true);
			if (item.itemType == "RawMaterial") {
				threeDimensionalPrefab.transform.position = worldPointAtGround + new Vector3(0, 4, 0);
			}
			else {
				threeDimensionalPrefab.transform.position = worldPointAtGround;
			}
			
		}
		else {
			//We are hovering over something that is not a droppable ground
			//Debug.Log("Oops we missed the ground");
			threeDimensionalPrefab.SetActive(false);
			canvasGroup.alpha = 0.75f; //Make it reappear
		}
		
	}

	public void OnEndDrag(PointerEventData eventData) {
		RectTransform rectTransformOfDraggableObj = eventData.pointerDrag.GetComponent<RectTransform>();
		//It looks like the OnDrop event handler doesn't even allow this to run
		
		/*
		//Here we need to check whether we are on top of the inventory UI before we decide anything else
		GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
		List<RaycastResult> results = new List<RaycastResult>();
		gr.Raycast(eventData, results);
		foreach (RaycastResult result in results) {
			Debug.Log(result.ToString());
		}
		*/

		Vector2 viewportCoordinates = getViewportCoordinatesOf2DRectTransform(rectTransformOfDraggableObj);

		//Send a ray through that viewport point, if we hit the ground layermask then we can continue, else we need to disable
		Ray ray = Camera.main.ViewportPointToRay(viewportCoordinates);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000f, GameManager.LAYER_DroppableGround)) { //Use bit shift by 3 to get 3rd layer
			//Then we hit something in the droppable ground
			//Get distance to hit and make that our distance from camera
			var worldPointAtGround = hit.point;

			//Now activate the prefab, remove one of these from inventory, and then destroy this
			threeDimensionalPrefab.SetActive(true);

			
			if (item.itemType == "RawMaterial") {
				threeDimensionalPrefab.transform.position = worldPointAtGround + new Vector3(0, 4, 0);
				//Let's make it pick-up able again
				IItem.makeClickCollectible2D(threeDimensionalPrefab);
				//Now enable the gravity
				threeDimensionalPrefab.GetComponent<Rigidbody>().useGravity = true;

			}
			else if (item.itemType == "Potion") {
				threeDimensionalPrefab.transform.position = worldPointAtGround;
				SpellAction spellActionScript = threeDimensionalPrefab.GetComponent<SpellAction>();
				spellActionScript.EnableSpell();
			}
			else {
				//Whoops how did we get here
			}

			InventoryManager.Instance.removeByID(itemID);
            InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges(); //Don't forget to update the inventory UI

            Destroy(gameObject);


		}
		else {
			//We are hovering over something that is not a droppable ground
			//Destroy the 3D prefab
			Destroy(threeDimensionalPrefab);
			//Update the inventory UI to restore changes and then delete this
			InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
			Destroy(gameObject);
		}


	}

}
