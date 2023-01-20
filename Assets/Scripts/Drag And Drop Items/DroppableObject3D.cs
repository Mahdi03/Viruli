using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This component will use the ItemInstance to get the itemID and necessary prefabs to instantiate
/// </summary>
[RequireComponent(typeof(ItemInstance))]
[RequireComponent(typeof(CanvasGroup))]
public class DroppableObject3D : MonoBehaviour, IDraggableObject2D {
	private Canvas canvas;
	private GameObject threeDimensionalItemsContainerForDraggingInWorldSpace;
	private CanvasGroup canvasGroup;
	private int itemID, attachedInventorySlotID;

	private IItem item;
	private GameObject threeDimensionalPrefab;

	private void Awake() {
		canvas = GameManager.Instance.mainCanvas; //Store the canvas in the scene so we can get its dimensions
		threeDimensionalItemsContainerForDraggingInWorldSpace = GameObject.FindGameObjectWithTag("threeDimensionalItemsContainerForDraggingInWorldSpace");

		canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null) {
			canvasGroup = (CanvasGroup)gameObject.AddComponent<CanvasGroup>();
		}
		ItemInstance itemInstance = GetComponent<ItemInstance>();
		this.itemID = itemInstance.itemID;
		this.attachedInventorySlotID = itemInstance.attachedInventorySlotID;
		this.item = InGameItemsDatabaseManager.Instance.getItemByID(itemID);
	}

	public void OnBeginDrag(PointerEventData eventData) {
		GameManager.Instance.midDrag = true;
		//Let's instantiate a new 3D spell that we can show and hide depending on where we are dragging
		if (threeDimensionalPrefab == null) {
			threeDimensionalPrefab = Instantiate(item.ThreeDimensionalPrefab);
			IItem.attachItemInstance(threeDimensionalPrefab, itemID);
			IItem.enableScript<OnHoverTooltip>(threeDimensionalPrefab);
			threeDimensionalPrefab.transform.SetParent(threeDimensionalItemsContainerForDraggingInWorldSpace.transform, true);

			//if we are dealing with a potion, resize the effect ring visually to match the radius
			//Debug.Log(item.itemType);
			if (item.itemType == "Potion") {
				float ringRadius = item.EffectRadius;
				threeDimensionalPrefab.transform.GetChild(2).transform.localScale = new Vector3(ringRadius, ringRadius, ringRadius);
			}
			//Debug.Log("resizing successful");
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
			x: rectTransform.position.x / canvasDimensions.x,
			y: rectTransform.position.y / canvasDimensions.y
			);
		return viewportCoordinates;
	}

	private Vector3 verticalOffsetFor3DDrop = new Vector3(0, 6, 0);

	private bool overInventoryUI;
	public void OnDrag(PointerEventData eventData) {
        GameManager.Instance.midDrag = true;
        RectTransform rectTransformOfDraggableObj = eventData.pointerDrag.GetComponent<RectTransform>();

		overInventoryUI = false;
		//Here we need to check whether we are on top of the inventory UI before we decide anything else
		GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();

		List<RaycastResult> results = new List<RaycastResult>();
		gr.Raycast(eventData, results);
		foreach (RaycastResult result in results) {
			if (result.gameObject.name.ToLower().Contains("inventory")) {
				overInventoryUI = true;
			}
			//Debug.Log(result.gameObject.name);
		}
		if (overInventoryUI) {
			//Then we need to disable the 3D one
			threeDimensionalPrefab.SetActive(false);
			canvasGroup.alpha = 0.75f; //Make it reappear

			//And then not do any of the rest of the 3D physics checking
			return;
		}

		Vector2 viewportCoordinates = getViewportCoordinatesOf2DRectTransform(rectTransformOfDraggableObj);

		//Send a ray through that viewport point, if we hit the ground layermask then we can continue, else we need to disable
		Ray ray = Camera.main.ViewportPointToRay(viewportCoordinates);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000f, GameManager.LAYER_DroppableGround)) {
			//Then we hit something in the droppable ground
			canvasGroup.alpha = 0f; //Make it completely transparent so that it APPEARS we are dragging the 3D object

			//Get distance to hit and make that our distance from camera
			var worldPointAtGround = hit.point;
			threeDimensionalPrefab.SetActive(true);
			if (item.GetType().IsSubclassOf(typeof(RawMaterial))) {
				threeDimensionalPrefab.transform.position = worldPointAtGround + verticalOffsetFor3DDrop;
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
		if (overInventoryUI) { //This value will linger on from the last OnDrag call
							   //We are hovering over something that is not a droppable ground
							   //Destroy the 3D prefab
			Destroy(threeDimensionalPrefab);
			//Update the inventory UI to restore changes and then delete this
			InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
			GameManager.Instance.midDrag = false;
			Destroy(gameObject);
			return;
		}

		Vector2 viewportCoordinates = getViewportCoordinatesOf2DRectTransform(rectTransformOfDraggableObj);

		//Send a ray through that viewport point, if we hit the ground layermask then we can continue, else we need to disable
		Ray ray = Camera.main.ViewportPointToRay(viewportCoordinates);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000f, GameManager.LAYER_DroppableGround)) {
			//Then we hit something in the droppable ground
			//Get distance to hit and make that our distance from camera
			var worldPointAtGround = hit.point;

			//Now activate the prefab, remove one of these from inventory, and then destroy this
			threeDimensionalPrefab.SetActive(true);
			//Debug.Log(item.GetType());
			//If we are dropping a raw material (whether it is for building or crafting)
			if (item.GetType().IsSubclassOf(typeof(RawMaterial))) {
				threeDimensionalPrefab.transform.position = worldPointAtGround + verticalOffsetFor3DDrop;
				
				/*//Let's make it pick-up able again
				IItem.enableScript<ClickAddInventory>(threeDimensionalPrefab);*/

				//make it do damage and get destroyed when it hits an enemy
				IItem.enableScript<Attack3DDroppableItem>(threeDimensionalPrefab);
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

            //Instead of removing by ID, we want to remove from that location
			//When we remove by ID it will remove the first instance of that object and then some weird glitch will happen where it comes back from another one
            //InventoryManager.Instance.removeByID(itemID);
			InventoryManager.Instance.removeAtSlotLocation(attachedInventorySlotID);
			InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges(); //Don't forget to update the inventory UI
			GameManager.Instance.midDrag = false;

			Destroy(gameObject);
		}
		else {
			//We are hovering over something that is not a droppable ground
			//Destroy the 3D prefab
			Destroy(threeDimensionalPrefab);
			//Update the inventory UI to restore changes and then delete this
			InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
			GameManager.Instance.midDrag = false;
			Destroy(gameObject);
		}
	}
}
