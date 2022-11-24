using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This component will use the ItemInstance to get the itemID and necessary prefabs to instantiate
/// </summary>
[RequireComponent(typeof(ItemInstance))]
public class DroppableObject3D : MonoBehaviour {
    private int itemID;
    private void Awake() {
        this.itemID = GetComponent<ItemInstance>().itemID;
    }



}
