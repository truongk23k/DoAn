using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy data/Range Weapon Data")]
public class Enemy_RangeWeaponData : ScriptableObject
{
    [Header("Weapon deatils")]
    public Enemy_RangeWeaponType weaponType;
    public float fireRate = 1; //bullets per second

    public int minBulletsPerAttack = 1;
    public int maxBulletsPerAttack = 1;

    public float minWeaponCooldown = 2f;
    public float maxWeaponCooldown = 3f;

    [Header("Bullet details")]
    public float bulletSpeed = 20f;
    public float weaponSpread = 0.1f;

    public int GetBulletsPerAttack() => Random.Range(minBulletsPerAttack, maxBulletsPerAttack + 1);

    public float GetWeaponCooldown() => Random.Range(minWeaponCooldown, maxWeaponCooldown);

    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {
        float randomizedValue = Random.Range(-weaponSpread, weaponSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

}
