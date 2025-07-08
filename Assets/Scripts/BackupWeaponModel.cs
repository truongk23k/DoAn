using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HangType
{
    LowBackHang,
    BackHang,
    SideHang
}
public class BackupWeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    [SerializeField] private HangType hangType;

    public void Activate(bool activate) => gameObject.SetActive(activate);

    public bool HangTypeIs(HangType hangType) => this.hangType == hangType; 
}
