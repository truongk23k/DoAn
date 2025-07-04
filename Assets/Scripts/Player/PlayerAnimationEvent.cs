using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerWeaponVisuals visualController;
    private PlayerWeaponController weaponController;

    private void Start()
    {
        visualController = GetComponentInParent<PlayerWeaponVisuals>();
        weaponController = GetComponentInParent<PlayerWeaponController>();
    }

    public void ReloadIsOver()
    {
        visualController.MaximizeRigWeight();

        weaponController.CurrentWeapon.RefillBullets();

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
