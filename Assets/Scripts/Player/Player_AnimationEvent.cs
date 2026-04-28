using UnityEngine;

public class Player_AnimationEvent : MonoBehaviour
{
    private Player_WeaponVisuals visualController;
    private Player_WeaponController weaponController;

    private void Start()
    {
        visualController = GetComponentInParent<Player_WeaponVisuals>();
        weaponController = GetComponentInParent<Player_WeaponController>();
    }

    public void ReloadIsOver()
    {
        visualController.MaximizeRigWeight();

        weaponController.CurrentWeapon().RefillBullets();

        weaponController.SetWeaponReady(true);
    }

    public void WeaponEquipIsOver()
    {
        weaponController.SetIsEquip(false);
        weaponController.SetWeaponReady(true);
    }

    public void ReturnRig()
    {
        visualController.MaximizeRigWeight();
        visualController.MaximizeLeftHandWeight();
    }

    public void ResetWeightRigAndLeftHandIK()
    {
        visualController.ResetWeightRigAndLeftHandIK();
    }

    public void SwitchOnWeaponModel() => visualController.SwitchOnCurrentWeaponModel();
}
