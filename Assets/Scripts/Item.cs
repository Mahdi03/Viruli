using System;
using System.Collections.Generic;
using UnityEngine;

//Place any methods in here you want access to inside of InventoryManager
public interface IItem {

	//Getters and setters
	public bool Stackable { get; } //Set through scriptable objects
	public bool Craftable { get; } //Set through scriptable objects
	
	[Serializable]
	public class recipeItem {
		public Item item;
		public int countRequired;
	}
	public recipeItem[] dirtyRecipe { get; } //Set through scriptable objects

	public List<(int, int)> Recipe { get; set; } //Actual recipe object that will be referenced throughout all scripts (set in InGameItemsDatabaseManager.cs)

	public int ID {
		get; set; //Allow settable for DatabaseManager
	}

	public abstract string itemType { get; } //Make it abstract so that we can override it later
	public abstract string itemName { get; }

	public abstract int spellLevel { get; }
	public int WeightedDropProbability { get; }
	public int XPValue { get; } //Both will have XP Values
	public abstract int XPCost { get; } //Only potions will have XP Cost

	public abstract float EffectRadius { get; }
	public abstract float EffectTimeout { get; }

	public abstract string ItemDescription { get; }

	public GameObject TwoDimensionalPrefab { get; } //Set thru editor
	public GameObject ThreeDimensionalPrefab { get; } //Set thru editor

	int inventorySlotIDOccupied { get; set; }
	public virtual void drop2DSprite(Vector2 pos, Quaternion rotation) { }
	public virtual void drop2DSprite(Vector3 pos, Quaternion rotation) { }

	/*Enabling and disabling scripts - static methods require definitions so they must be defined right away*/


    //Works with both 2D and 3D prefabs
    public static void attachItemInstance(GameObject prefab, int itemID, int attachedInventorySlotID = -1) {
        ItemInstance itemInstance = prefab.GetComponent<ItemInstance>();
        if (itemInstance == null) {
            itemInstance = prefab.AddComponent<ItemInstance>();
        }
        itemInstance.itemID = itemID; //Give prefab the item ID that it corresponds to
        itemInstance.attachedInventorySlotID = attachedInventorySlotID;
    }
    public static void enableScript<T>(GameObject prefab) {
        T script = prefab.GetComponent<T>();
        if (script == null) {
            prefab.AddComponent(typeof(T)); //This line is problematic...why?
        }
        else {
            //We have to do this long workaround because since the type is unknown and generic, it could also not have the "enabled" property we are trying to access
            if (script.GetType().GetProperty("enabled") != null) {
                script.GetType().GetProperty("enabled").SetValue(script, true);
            }
        }
    }
    public static void disableScript<T>(GameObject prefab) {
        T script = prefab.GetComponent<T>();
        if (script != null) {
            //We have to do this long workaround because since the type is unknown and generic, it could also not have the "enabled" property we are trying to access
            if (script.GetType().GetProperty("enabled") != null) {
                script.GetType().GetProperty("enabled").SetValue(script, false);
            }
        }
    }

}







public class Item : ScriptableObject, IItem {
	/*
	 * Use SerializeField and an explicit private variable to expose these values to the Unity editor since they don't 
	 * support the automatic property thingy yet
	 */
	[SerializeField]
	private bool stackable = true;
	public bool Stackable { get { return stackable; } }
	[SerializeField]
	private bool craftable = false;
	public bool Craftable { get { return craftable; } }

	[SerializeField]
	private IItem.recipeItem[] myrecipe;
	public IItem.recipeItem[] dirtyRecipe { get { return myrecipe; } }
	public List<(int, int)> Recipe { get; set; }

	public virtual float EffectRadius { get { return -1f; } }
	public virtual float EffectTimeout { get { return -1f; } }

	[SerializeField]
	private int xpValue = 0;
	public int XPValue { get => xpValue; }
	public virtual int XPCost { get; }

	[SerializeField]
	private int weightedDropProbability = 0;
    public int WeightedDropProbability { get => weightedDropProbability; }

    [SerializeField]
	private GameObject twoDimensionalPrefab;
	public GameObject TwoDimensionalPrefab { get { return twoDimensionalPrefab; } }
	[SerializeField]
	private GameObject threeDimensionalPrefab;
	public GameObject ThreeDimensionalPrefab { get { return threeDimensionalPrefab; } }

	[SerializeField]
	private string itemDescription;
	virtual public string ItemDescription { get { return itemDescription; } }

	virtual public string itemType { get; }
	public string itemName { get { return this.name; } }

	virtual public int spellLevel { get; }

	//[SerializeField] - We don't set this ourselves because it is set by the DatabaseManager
	private int itemID;
	public int ID {
		get { return itemID; }
		set { itemID = value; } //We need the setter because it is set by the DatabaseManager
	}

	public int inventorySlotIDOccupied { get; set; } = -1;
	
	protected bool currently2D;
	
	private static float canvasScale = -1f;
	private static Vector2 canvasDimensions = Vector2.zero;
	
	public virtual void drop2DSprite(Vector3 pos, Quaternion rotation) {

        //TODO: Convert pos to 2-D screen coordinates and then call our drop function
        var screenSpaceCoordinates = Camera.main.WorldToScreenPoint(pos);
		/* drop2DSprite takes a 2D vector from the anchored position which is the center
		 * we need to first convert the screen coordinates to centered and unscaled values
		 */
		
		//Get the canvas scale factor
		if (canvasScale == -1f) {
			//This means we haven't changed it yet so let's change it
			Canvas canvas = GameManager.Instance.mainCanvas;

            canvasScale = canvas.scaleFactor;
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            canvasDimensions = new Vector2(canvasRectTransform.rect.width, canvasRectTransform.rect.height) * canvasScale;
        }
		var normalizedScreenSpaceCoordinates = new Vector2(screenSpaceCoordinates.x - canvasDimensions.x/2, screenSpaceCoordinates.y - canvasDimensions.y/2) / canvasScale;

		//vary the points a little bit so that they aren't all direct stacked on one another
		var newPos = new Vector2(normalizedScreenSpaceCoordinates.x, normalizedScreenSpaceCoordinates.y) + UnityEngine.Random.insideUnitCircle * 7.5f;
		//Ignoring the Z for now hopefully it doesn't make too much of a difference

        drop2DSprite(newPos, rotation);
		
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
		IItem.attachItemInstance(newSprite, ID); //Send it just the ID, we don't need to send it all the details
		IItem.enableScript<ClickAddInventory>(newSprite);
		IItem.enableScript<ItemFloat>(newSprite);
		IItem.disableScript<DraggableObject2D>(newSprite);
        IItem.disableScript<OnHoverTooltip>(newSprite);
		var newSpriteRectTransform = newSprite.GetComponent<RectTransform>();
		newSpriteRectTransform.anchoredPosition = pos;
	}


}
