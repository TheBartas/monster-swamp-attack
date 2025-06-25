using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="HandWeapon", menuName="Weapon/HandWeapon")]
public class HandWeaponData : ScriptableObject {
    
    [Header("Info")]
    [SerializeField] public new string name;
    [SerializeField] public int dmg;
    [SerializeField] public float useStamina;
}
