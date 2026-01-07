using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_WeaponModel : MonoBehaviour
{
    public Enemy_MeleeWeaponType weaponType;
    public GameObject weaponHidden;
    public AnimatorOverrideController overrideController;

    [SerializeField] private GameObject[] trailEffects;

    /*private void Awake()
    {
        EnableTrailEffect(false);
    }*/

    public void EnableTrailEffect(bool enable)
    {
        foreach(var trail in trailEffects)
        {
            trail.SetActive(enable);
        }
    }
}
