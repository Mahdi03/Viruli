using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Raw Materials/Create New Building Material")]
public class BuildingMaterial : RawMaterial {
    public override string itemType { get { return this.GetType().Name; } }
}
