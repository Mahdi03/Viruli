using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemInstance {
    int itemID { get; set; }
    int attachedInventorySlot { get; set; }
}

public class ItemInstance : MonoBehaviour, IItemInstance {
    public int itemID { get; set; }
    public int attachedInventorySlot { get; set; }
}
