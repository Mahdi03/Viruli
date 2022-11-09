using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : DraggableObject {
	public const bool stackable = false;
	public const int xpValue = 0;
	public GameObject
		twoDimensionalPrefab,
		threeDimensionalPrefab;
	protected string itemType;
	public int itemID;

	protected bool currently2D;
	public void switch2Dto3DPrefab() { }
	public void switch3Dto2DPrefab() { }


	public virtual void showOnSceneRing() { }
}
