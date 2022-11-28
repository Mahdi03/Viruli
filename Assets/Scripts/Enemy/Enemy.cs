using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Create New Enemy")]
public class Enemy : ScriptableObject {
	public int enemyID;
	public GameObject enemyPrefab;
	public float speed;
	public int maxHealth;
	public int dealsDamage;
	public int xpValue;
	public int minItemDropCount, maxItemDropCount;
	public int spawnProbability;
}
