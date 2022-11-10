using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject {
	public bool stackable = false;
	public int XPValue = 0;
	public GameObject
		twoDimensionalPrefab,
		threeDimensionalPrefab;
	protected string itemType;
	public int itemID;

	protected bool currently2D;
	public void switch2Dto3DPrefab() { }
	public void switch3Dto2DPrefab() { }


	public virtual void showOnSceneRing() { }

	//We can attach script components to the 2D and 3D prefabs so that when we want access to their Item class we can access them here
	void init2DGameObject() {
		twoDimensionalPrefab.AddComponent<ItemScriptableObjectCommunicator>();
		twoDimensionalPrefab.AddComponent<DraggableObject2D>();
	}
	void init3DGameObject() {
		threeDimensionalPrefab.AddComponent<ItemScriptableObjectCommunicator>();
		threeDimensionalPrefab.AddComponent<DraggableObject3D>();
	}
	
}

public class ItemScriptableObjectCommunicator : MonoBehaviour {

}