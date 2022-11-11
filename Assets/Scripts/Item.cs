using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Place any methods in here you want access to inside of InventoryManager
public interface IItem {

	//Getters and setters
	public bool GetStackable();
	public int GetItemID();
	public string GetItemType();
	public GameObject Get2DPrefab();
	public GameObject Get3DPrefab();

	public int GetInventorySlotIDOccupied();
	public void SetInventorySlotIDOccupied(int id);
	public virtual void init2DGameObject() { }
	public virtual void init3DGameObject() { }
	public virtual void drop2DSprite(Vector2 pos, Quaternion rotation) { }
	void SetCurrent2DPrefab(GameObject item2DPrefab);
	public virtual void drop3DSprite(Vector3 worldPos, Quaternion rotation) { }

	/* Enabling and disabling scripts */
	public static void makeDraggable2D(GameObject twoDimensionalPrefab) {}
	public static void disableDraggable2D(GameObject twoDimensionalPrefab) {}
	public static void makeHoverFloat2D(GameObject twoDimensionalPrefab) {}
	public static void disableHoverFloat2D(GameObject twoDimensionalPrefab) {}
	public static void makeClickCollectible2D(GameObject twoDimensionalPrefab) {}
	public static void disableClickCollectible2D(GameObject twoDimensionalPrefab) {}

}







public class Item : ScriptableObject, IItem {
	public bool stackable = false;
	public bool GetStackable() { return stackable; }
	public int XPValue = 0;
	public GameObject
		twoDimensionalPrefab,
		threeDimensionalPrefab;
	public GameObject Get2DPrefab() { return twoDimensionalPrefab; }
	public void SetCurrent2DPrefab(GameObject item2DPrefab) {
		this.twoDimensionalPrefab = item2DPrefab;
	}
	public GameObject Get3DPrefab() { return threeDimensionalPrefab; }
	public string itemType;
	public string GetItemType() { return itemType; }
	public int itemID;
	public int GetItemID() { return itemID; }

	private int inventorySlotIDOccupied = -1;
	public int GetInventorySlotIDOccupied() {
		return inventorySlotIDOccupied;
	}
	public void SetInventorySlotIDOccupied(int id) {
		inventorySlotIDOccupied = id;
	}
	/*
	protected bool
		twoDimensionalPrefabInitialized = false,
		threeDimensionalPrefabInitialized = false;
	*/

	protected bool currently2D;
	public void switch2Dto3DPrefab() { }
	public void switch3Dto2DPrefab() { }



	public virtual void showOnSceneRing() { }

	//We can attach script components to the 2D and 3D prefabs so that when we want access to their Item class we can access them here
	public virtual void init2DGameObject() {
		AttachedItemData itemData = twoDimensionalPrefab.GetComponent<AttachedItemData>();
		if (itemData == null) {
			itemData = twoDimensionalPrefab.AddComponent<AttachedItemData>();
		}
		//Everything inherited from Item
		itemData.stackable = this.stackable;
		itemData.XPValue = this.XPValue;
		itemData.twoDimensionalPrefab = this.twoDimensionalPrefab;
		itemData.threeDimensionalPrefab = this.threeDimensionalPrefab;
		itemData.itemType = this.itemType;
		itemData.itemID = this.itemID;
		itemData.SetInventorySlotIDOccupied(this.inventorySlotIDOccupied);
		

		//this.twoDimensionalPrefabInitialized = true;
	}
	/*
	public void enableScript<Unity.Component<T>>() {
		T script = twoDimensionalPrefab.GetComponent<T>();
		if (script == null) {
			twoDimensionalPrefab.AddComponent<T>(); //This line is problematic...why?
		}
		else {
			script.enabled = true;
		}
	}
	public void disableScript<T>() {
		T script = myPrefab.GetComponent<T>();
		if (script != null) {
			script.enabled = false;
		}
	}
	*/

	public static void makeDraggable2D(GameObject twoDimensionalPrefab) {
		DraggableObject2D draggableObject2DScript = twoDimensionalPrefab.GetComponent<DraggableObject2D>();
		if (draggableObject2DScript == null) {
			twoDimensionalPrefab.AddComponent<DraggableObject2D>();
		}
		else {
			draggableObject2DScript.enabled = true;
		}
	}
	public static void disableDraggable2D(GameObject twoDimensionalPrefab) {
		DraggableObject2D draggableObject2DScript = twoDimensionalPrefab.GetComponent<DraggableObject2D>();
		if (draggableObject2DScript != null) {
			draggableObject2DScript.enabled = false;
		}
	}

	public static void makeHoverFloat2D(GameObject twoDimensionalPrefab) {
		ItemHover itemHoverScript = twoDimensionalPrefab.GetComponent<ItemHover>();
		if (itemHoverScript == null) {
			twoDimensionalPrefab.AddComponent<ItemHover>();
		}
		else {
			itemHoverScript.enabled = true;
		}
	}
	public static void disableHoverFloat2D(GameObject twoDimensionalPrefab) {
		ItemHover itemHoverScript = twoDimensionalPrefab.GetComponent<ItemHover>();
		if (itemHoverScript != null) {
			itemHoverScript.enabled = false;
		}
	}
	public static void makeClickCollectible2D(GameObject twoDimensionalPrefab) {
		ClickAddInventory clickAddInventoryScript = twoDimensionalPrefab.GetComponent<ClickAddInventory>();
		if (clickAddInventoryScript == null) {
			twoDimensionalPrefab.AddComponent<ClickAddInventory>();
		}
		else {
			clickAddInventoryScript.enabled = true;
		}
	}
	public static void disableClickCollectible2D(GameObject twoDimensionalPrefab) {
		ClickAddInventory clickAddInventoryScript = twoDimensionalPrefab.GetComponent<ClickAddInventory>();
		if (clickAddInventoryScript != null) {
			clickAddInventoryScript.enabled = false;
		}

	}
	/**
	 * Call this function at the location of a zombie death to drop a 2-D collectible item
	 */
	public virtual void drop2DSprite(Vector2 pos, Quaternion rotation) {
		//if (!this.twoDimensionalPrefabInitialized) {
			init2DGameObject();
		//}
		
		Transform twoDimensionalSpritesContainer = GameObject.FindGameObjectWithTag("2DItemsContainerInCanvas").transform;
		var newSprite = Instantiate(twoDimensionalPrefab, new Vector2(0, 0), rotation, twoDimensionalSpritesContainer);
        makeClickCollectible2D(newSprite);
        makeHoverFloat2D(newSprite);
        disableDraggable2D(newSprite);
        var newSpriteRectTransform = newSprite.GetComponent<RectTransform>();
		newSpriteRectTransform.anchoredPosition = pos;
	}


	public virtual void drop3DSprite(Vector3 worldPos, Quaternion rotation) {
		throw new System.NotImplementedException();
	}

	public virtual void init3DGameObject() {
		AttachedItemData itemData = twoDimensionalPrefab.GetComponent<AttachedItemData>();
		if (itemData == null) {
			itemData = twoDimensionalPrefab.AddComponent<AttachedItemData>();
		}
		//Everything inherited from Item
		itemData.stackable = this.stackable;
		itemData.XPValue = this.XPValue;
		itemData.twoDimensionalPrefab = this.twoDimensionalPrefab;
		itemData.threeDimensionalPrefab = this.threeDimensionalPrefab;
		itemData.itemType = this.itemType;
		itemData.itemID = this.itemID;
		itemData.SetInventorySlotIDOccupied(this.inventorySlotIDOccupied);


		//this.threeDimensionalPrefabInitialized = true;
		
	}

}

public class EmptyItem: Item {
	public EmptyItem() {
		this.itemType = null;
	}
}