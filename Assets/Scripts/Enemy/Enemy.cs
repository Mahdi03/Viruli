using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Create New Enemy")]
public class Enemy : ScriptableObject {
    public GameObject enemyPrefab;
    public float speed;
    public int maxHealth;
    public int dealsDamage;
}
