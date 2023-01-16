using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Using ScriptableObject as a layer of abstraction/encapsulation so that the items are stored in a file instead of
 * directly on the object in case we need to move it around or accidental deletion
 */

[CreateAssetMenu(menuName = "Items/Create GameItemsDatabase")]
public class GameItemsDatabase : ScriptableObject {
	public List<Potion> potions;
	//public List<RawMaterial> rawMaterials;
    public List<BuildingMaterial> buildingMaterials;
    public List<CraftingMaterial> craftingMaterials;
	public List<Enemy> enemies;
	[SerializeField]	
	public MainDoorsForEachLayout mainDoorsForEachLayout; //Each element corresponds to a list of main doors in a level
}

/* All this stupid class systeming just because Unity inspector doesn't want to render a list of a list*/

[Serializable]
public class MainDoorsForEachLayout {
	public List<MainDoorsList> mainDoorsForEachLayout; //Each element corresponds to a list of main doors in a level
}

[Serializable]
public class MainDoorsList {
    public List<MainDoor> mainDoorsList; //Each element corresponds to a main door in the scene for the respective level
}