using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemInstance {
    int itemID { get; set; }
}

public class ItemInstance : MonoBehaviour, IItemInstance {
    public int itemID { get; set; }
}
