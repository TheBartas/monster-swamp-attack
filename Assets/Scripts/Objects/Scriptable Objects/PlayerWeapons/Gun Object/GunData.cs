using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Gun", menuName="Weapon/Gun")]
public class GunData : ScriptableObject {

    [Header("Info")]
    public new string name;

    [Header("Shooting")]
    public float recoilForce;
    public int damage;
    public int range;

    [Header("Reloading")]
    public int currentAmmo;
    public float timeBetweenShooting;
    public float spread;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize;
    public int bulletsPerTap;

    [Header("Levels")]
    public int damageLevel;
    public int rangeLevel;
}
