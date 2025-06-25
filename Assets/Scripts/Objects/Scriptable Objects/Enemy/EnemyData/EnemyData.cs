using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Enemy", menuName="Enemy")]
public class EnemyData : ScriptableObject {
    [Header("Info")]
    [SerializeField] public string enemyName;
    [SerializeField] public float health;
    [SerializeField] public float dmg;
    [SerializeField] public float speed;
    [SerializeField] public float charge;
    [SerializeField] public int waterSpeed;
}
