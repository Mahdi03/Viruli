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
	public List<RawMaterial> rawMaterials;
	public List<Enemy> enemies;
}