using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Raw Material")]
public class RawMaterial : Item {
    public override string itemType { get { return this.GetType().Name; } }


}
