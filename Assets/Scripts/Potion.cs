using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Potion")]
public class Potion : Item {
	public Potion() {
		this.itemType = "Potion";
	}
    public GameObject effectRingPrefab;
    public int effectRadius;
    public float timeout = 5f;
    public List<(Item, int)> recipe;

    public void init(AttachedPotionData attachedPotionData) {
        //Everything inherited from Item
        this.stackable = attachedPotionData.stackable;
        this.XPValue = attachedPotionData.XPValue;
        this.twoDimensionalPrefab = attachedPotionData.twoDimensionalPrefab;
        this.threeDimensionalPrefab = attachedPotionData.threeDimensionalPrefab;

        //Properties specific to Potion
        this.itemType = attachedPotionData.itemType;
        this.effectRingPrefab = attachedPotionData.effectRingPrefab;
        this.effectRadius = attachedPotionData.effectRadius;
        this.timeout = attachedPotionData.timeout;
        this.recipe = attachedPotionData.recipe;
    }

    public override void showOnSceneRing() {
		base.showOnSceneRing();
	}
    public override void init2DGameObject() {
        base.init2DGameObject();
        AttachedItemData itemData = twoDimensionalPrefab.GetComponent<AttachedItemData>();
        AttachedPotionData potionData = twoDimensionalPrefab.GetComponent<AttachedPotionData>();
        if (potionData == null) {
            potionData = twoDimensionalPrefab.AddComponent<AttachedPotionData>();
        }
        Debug.Log("Called in Potion.cs");
        potionData.init(itemData, this.effectRingPrefab, this.effectRadius, this.timeout, this.recipe);
        itemData.enabled = false;
        Destroy(itemData);

        //this.twoDimensionalPrefabInitialized = true;
    }
    public override void drop2DSprite(Vector2 pos, Quaternion rotation) {
        //base.drop2DSprite(pos, rotation);
        Debug.Log("We made it this far");
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
        //this.SetCurrent2DPrefab(newSprite); //Trying something out
    }
    public override void drop3DSprite(Vector3 worldPos, Quaternion rotation) {
        base.drop3DSprite(worldPos, rotation);
    }
    public override void init3DGameObject() {
        base.init3DGameObject();
        AttachedItemData itemData = threeDimensionalPrefab.GetComponent<AttachedItemData>();
        AttachedPotionData potionData = threeDimensionalPrefab.GetComponent<AttachedPotionData>();
        if (potionData == null) {
            potionData = threeDimensionalPrefab.AddComponent<AttachedPotionData>();
        }
        potionData.init(itemData, this.effectRingPrefab, this.effectRadius, this.timeout, this.recipe);
        itemData.enabled = false;
        Destroy(itemData);
        //this.threeDimensionalPrefabInitialized = true;
    }

}
