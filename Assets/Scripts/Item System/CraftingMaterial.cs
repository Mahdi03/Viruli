using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Raw Materials/Create New Crafting Material")]
public class CraftingMaterial : RawMaterial {
    public override string itemType { get { return this.GetType().Name; } }
}
