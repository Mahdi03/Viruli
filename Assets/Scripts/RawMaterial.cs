using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Raw Material")]
public class RawMaterial : Item {
	public bool droppedByEnemy;
    /*
	public RawMaterial() {
		this.itemType = "rawMaterial";
	}
    */
    public override string itemType { get { return this.GetType().Name; } }
    /*
    public void init(AttachedRawMaterialData attachedRawMaterialData) {
        //Everything inherited from Item
        this.stackable = attachedRawMaterialData.stackable;
        this.XPValue = attachedRawMaterialData.XPValue;
        this.twoDimensionalPrefab = attachedRawMaterialData.twoDimensionalPrefab;
        this.threeDimensionalPrefab = attachedRawMaterialData.threeDimensionalPrefab;

        //Properties specific to RawMaterial
        this.droppedByEnemy = attachedRawMaterialData.droppedByEnemy;
    }
    */

    public override void showOnSceneRing() {
		base.showOnSceneRing();
		//Show a small signal on the scene that we can drop our 3-D object there
	}

    /*
    public override void init2DGameObject() {
        base.init2DGameObject();
        AttachedItemData itemData = twoDimensionalPrefab.GetComponent<AttachedItemData>();
        AttachedRawMaterialData rawMaterialData = twoDimensionalPrefab.GetComponent<AttachedRawMaterialData>();
        if (rawMaterialData == null) {
            rawMaterialData = twoDimensionalPrefab.AddComponent<AttachedRawMaterialData>();
        }
        rawMaterialData.init(itemData, this.droppedByEnemy);
        Destroy(itemData);

        //this.twoDimensionalPrefabInitialized = true;
    }
    public override void init3DGameObject() {
        base.init3DGameObject();
        AttachedItemData itemData = threeDimensionalPrefab.GetComponent<AttachedItemData>();
        AttachedRawMaterialData rawMaterialData = threeDimensionalPrefab.GetComponent<AttachedRawMaterialData>();
        if (rawMaterialData == null) {
            rawMaterialData = threeDimensionalPrefab.AddComponent<AttachedRawMaterialData>();
        }
        rawMaterialData.init(itemData, this.droppedByEnemy);
        Destroy(itemData);
        //this.threeDimensionalPrefabInitialized = true;
    }
    */
}
