using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
public class Weapon_Data : ScriptableObject
{
    public string weaponName;

    [Header("Magazine details")]
    public int bulletInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Header("Regular shoot")]
    public ShootType shootType;
    public int bulletPerShoot = 1;
    public float fireRate;

    [Header("Burst shoot")]
    public bool burstAvalible;
    public bool burstActive;
    public int burstBulletsPerShoot;
    public float burstFireRate;
    public float burstFireDelay = 0.1f; // time between two shoot

    [Header("Weapon Spread")]
    public float baseSpread;
    public float maxSpread;
    public float spreadIncreaseRate = 0.15f;
    public float spreadCooldown = 0.6f; //default guns

    [Header("Weapon generics")]
    public WeaponType weaponType;
    [Range(1, 3)]
    public float reloadSpeed = 1;
    [Range(1, 3)]
    public float equipmentSpeed = 1;
    [Range(4, 8)]
    public float gunDistance = 4;
    [Range(4, 8)]
    public float cameraDistance = 6;

}
