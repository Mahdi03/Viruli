using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Place any methods in here you want access to inside of InventoryManager
public interface IItem {

	//Getters and setters
	public bool Stackable { get; } //Set through scriptable objects
	public int ID {
		get; set; //Allow settable for DatabaseManager
	}

	public abstract string itemType { get; } //Make it abstract so that we can override it later
	public abstract string itemName { get; }

	public GameObject TwoDimensionalPrefab { get; } //Set thru editor
	public GameObject ThreeDimensionalPrefab { get; } //Set thru editor

	int inventorySlotIDOccupied { get; set; }
	public virtual void drop2DSprite(Vector2 pos, Quaternion rotation) { }
	/*
	void SetCurrent2DPrefab(GameObject item2DPrefab);
	*/
	public virtual void drop3DSprite(Vector3 worldPos, Quaternion rotation) { }

	/* Enabling and disabling scripts */
	public static void attachItemInstance(GameObject twoDimensionalPrefab, int itemID) {}
	public static void makeDraggable2D(GameObject twoDimensionalPrefab) {}
	public static void disableDraggable2D(GameObject twoDimensionalPrefab) {}
	public static void makeItemFloat2D(GameObject twoDimensionalPrefab) {}
	public static void disableItemFloat2D(GameObject twoDimensionalPrefab) {}
	public static void makeClickCollectible2D(GameObject twoDimensionalPrefab) {}
	public static void disableClickCollectible2D(GameObject twoDimensionalPrefab) {}

}







public class Item : ScriptableObject, IItem {
	/*
	 * Use SerializeField and an explicit private variable to expose these values to the Unity editor since they don't 
	 * support the automatic property thingy yet
	 */
	[SerializeField]
	private bool stackable;
	public bool Stackable { get { return stackable; } }


	public int XPValue = 0;

	[SerializeField]
	private GameObject twoDimensionalPrefab;
	public GameObject TwoDimensionalPrefab { get { return twoDimensionalPrefab; } }
	[SerializeField]
	private GameObject threeDimensionalPrefab;
	public GameObject ThreeDimensionalPrefab { get { return threeDimensionalPrefab; } }
	
	virtual public string itemType { get; }
	public string itemName { get { return this.name; } }

	//[SerializeField] - We don't set this ourselves because it is set by the DatabaseManager
	private int itemID;
	public int ID {
		get { return itemID; }
		set { itemID = value; } //We need the setter because it is set by the DatabaseManager
	}

	public int inventorySlotIDOccupied { get; set; } = -1;
	
	protected bool currently2D;

	public virtual void showOnSceneRing() { }

	/*
	public void enableScript<T>() {
		T script = twoDimensionalPrefab.GetComponent<T>();
		if (script == null) {
			twoDimensionalPrefab.AddComponent(T); //This line is problematic...why?
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
	//Works with both 2D and 3D prefabs
	public static void attachItemInstance(GameObject prefab, int itemID, int attachedInventorySlotID = -1) {
		ItemInstance itemInstance = prefab.GetComponent<ItemInstance>();
		if (itemInstance == null) {
			itemInstance = prefab.AddComponent<ItemInstance>();
		}
		itemInstance.itemID = itemID; //Give prefab the item ID that it corresponds to
		itemInstance.attachedInventorySlotID = attachedInventorySlotID;
	}
	//Make sure this function is called after attachItemInstance()
	public static void allowHoverTooltip(GameObject prefab) {
		OnHoverTooltip onHoverTooltip = prefab.GetComponent<OnHoverTooltip>();
        if (onHoverTooltip == null) {
            prefab.AddComponent<OnHoverTooltip>();
        }
        else {
            onHoverTooltip.enabled = true;
        }
    }
	public static void disallowHoverTooltip(GameObject prefab) {
        OnHoverTooltip onHoverTooltip = prefab.GetComponent<OnHoverTooltip>();
        if (onHoverTooltip != null) {
            onHoverTooltip.enabled = false;
        }
    }
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

	public static void makeItemFloat2D(GameObject twoDimensionalPrefab) {
		ItemFloat itemFloatScript = twoDimensionalPrefab.GetComponent<ItemFloat>();
		if (itemFloatScript == null) {
			twoDimensionalPrefab.AddComponent<ItemFloat>();
		}
		else {
			itemFloatScript.enabled = true;
		}
	}
	public static void disableItemFloat2D(GameObject twoDimensionalPrefab) {
		ItemFloat itemFloatScript = twoDimensionalPrefab.GetComponent<ItemFloat>();
		if (itemFloatScript != null) {
			itemFloatScript.enabled = false;
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
		/*
		//if (!this.twoDimensionalPrefabInitialized) {
			init2DGameObject();
		//}
		*/
		Transform twoDimensionalSpritesDroppingContainer = GameObject.FindGameObjectWithTag("2DItemsContainerForDroppingItemsInCanvas").transform;
		var newSprite = Instantiate(TwoDimensionalPrefab, new Vector2(0, 0), rotation, twoDimensionalSpritesDroppingContainer);
		attachItemInstance(newSprite, ID); //Send it just the ID, we don't need to send it all the details
		makeClickCollectible2D(newSprite);
		makeItemFloat2D(newSprite);
		disableDraggable2D(newSprite);
		disallowHoverTooltip(newSprite);
		var newSpriteRectTransform = newSprite.GetComponent<RectTransform>();
		newSpriteRectTransform.anchoredPosition = pos;
	}


	public virtual void drop3DSprite(Vector3 worldPos, Quaternion rotation) {
		throw new System.NotImplementedException();
	}

}
