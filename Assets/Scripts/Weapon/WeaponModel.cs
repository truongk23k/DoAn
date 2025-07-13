using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipType
{
    SideEquipAnimation,
    BackEquipAnimation
}

public enum HoldType
{
    CommonHold = 1,
    LowHold,
    HighHold
}

public class WeaponModel : MonoBehaviour
{
    public Weapon_Data weaponData_Model;
    

    public Transform gunPoint;
    public Transform holdPoint;

    private void Start()
    {
        UpdateGameObject();
    }

    [ContextMenu("Update Model in hand")]
    public void UpdateGameObject()
    {
        gameObject.name = "Model_Weapon - " + weaponData_Model.weaponName.ToString() + " - equip" + (int)weaponData_Model.equipAnimationType + " - hold" + weaponData_Model.holdType;
    }
}
