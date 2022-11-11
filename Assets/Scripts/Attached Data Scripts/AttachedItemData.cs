using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachedItemData : MonoBehaviour {
	public bool stackable = false;
	public int XPValue = 0;
	public GameObject
		twoDimensionalPrefab,
		threeDimensionalPrefab;
	public string itemType;
	public int itemID;

	protected int inventorySlotIDOccupied;
    public int GetInventorySlotIDOccupied() {
        return inventorySlotIDOccupied;
    }
    public void SetInventorySlotIDOccupied(int id) {
        inventorySlotIDOccupied = id;
    }


    protected bool currently2D;

	/*
	public AttachedItemData(bool stackable, int xPValue, GameObject twoDimensionalPrefab, GameObject threeDimensionalPrefab, string itemType, int itemID, bool currently2D) {
		this.stackable = stackable;
		this.XPValue = xPValue;
		this.twoDimensionalPrefab = twoDimensionalPrefab;
		this.threeDimensionalPrefab = threeDimensionalPrefab;
		this.itemType = itemType;
		this.itemID = itemID;
		this.currently2D = currently2D;
	}*/
}
