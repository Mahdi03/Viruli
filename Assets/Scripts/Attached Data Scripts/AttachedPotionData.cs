using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachedPotionData : AttachedItemData {

    public GameObject effectRingPrefab;
    public int effectRadius;
	public float timeout = 5f;
	public List<(Item, int)> recipe;

    public void init(AttachedItemData attachedItemData, GameObject ringPrefab,
        int potionEffectRadius, float potionTimeout, List<(Item, int)> potionRecipe) {
        //This will copy over all the values from the base class
        this.stackable = attachedItemData.stackable;
        this.XPValue = attachedItemData.XPValue;
        this.twoDimensionalPrefab = attachedItemData.twoDimensionalPrefab;
        this.threeDimensionalPrefab = attachedItemData.threeDimensionalPrefab;
        this.itemType = attachedItemData.itemType;
        this.itemID = attachedItemData.itemID;
        this.inventorySlotIDOccupied = attachedItemData.GetInventorySlotIDOccupied();

        //Add the new values for this specific class
        this.effectRingPrefab = ringPrefab;
        this.effectRadius = potionEffectRadius;
        this.timeout = potionTimeout;
        this.recipe = potionRecipe;
    }

    public Potion toPotionClass() {
        Potion thisPotion = new Potion();
        thisPotion.init(this);
        return thisPotion;
    }
}
