using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerWeaponVisuals visualController;

    private void Start()
    {
        visualController = GetComponentInParent<PlayerWeaponVisuals>();
    }

    public void ReloadIsOver()
    {
        Debug.Log("reload done");
        visualController.MaximizeRigWeight();

        //refill bullets
    }

    public void WeaponGrabIsOver()
    {
        visualController.SetBusyGrabbingWeaponTo(false);
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
}
